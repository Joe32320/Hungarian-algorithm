using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungarianAl<T> {

    List<T> supplyVertices;
    List<T> demandVertices;

    Dictionary<T, Dictionary<T, int>> costDict;
    Dictionary<T, Dictionary<T, int>> workedCostDict;

    Dictionary<T, Dictionary<T, bool>> partialGraph;

    Dictionary<T, Dictionary<T, int>> edgeWeights;

    Dictionary<T, int> supplyWeights;
    Dictionary<T, int> demandWeights;

    Dictionary<T, int> supplyAmount;
    Dictionary<T, int> demandAmount;

    Dictionary<T, int> labelled;
    Dictionary<T, bool> newlyLabelled;

    

    Dictionary<T, int> vertexIDs;
    Dictionary<int, T> IDsToVertex;



    public HungarianAl(List<T> nSupplyVertices, List<T> nDemandVertices, Dictionary<T, Dictionary<T, int>> nCostDict, Dictionary<T, int> nSupplyAmount, Dictionary<T, int> nDemandAmount)
    {

        supplyVertices = nSupplyVertices;
        demandVertices = nDemandVertices;
        costDict = nCostDict;
        supplyAmount = nSupplyAmount;
        demandAmount = nDemandAmount;
        edgeWeights = new Dictionary<T, Dictionary<T, int>>();
        newlyLabelled = new Dictionary<T, bool>();

        partialGraph = new Dictionary<T, Dictionary<T, bool>>();
        labelled = new Dictionary<T, int>();
        for (int i = 0; i < supplyVertices.Count; i++)
        {
            Dictionary<T, bool> dict = new Dictionary<T, bool>();
            Dictionary<T, int> dictE = new Dictionary<T, int>();
            for (int j = 0; j < demandVertices.Count; j++)
            {
                dict.Add(demandVertices[j], false);
                dictE.Add(demandVertices[j], 0);

                if (!labelled.ContainsKey(demandVertices[j]))
                {
                    labelled.Add(demandVertices[j], -1);
                    newlyLabelled.Add(demandVertices[j], false);
                }

            }

            partialGraph.Add(supplyVertices[i], dict);
            edgeWeights.Add(supplyVertices[i], dictE);
            labelled.Add(supplyVertices[i], -1);
            newlyLabelled.Add(supplyVertices[i], false);

        }

        vertexIDs = new Dictionary<T, int>();
        IDsToVertex = new Dictionary<int, T>();

        for (int i = 0; i < supplyVertices.Count + demandVertices.Count; i++)
        {
            if (i < supplyVertices.Count)
            {
                vertexIDs.Add(supplyVertices[i], i + 1);
                IDsToVertex.Add(i + 1, supplyVertices[i]);
                Debug.Log(supplyVertices[i].ToString() + ":" + vertexIDs[supplyVertices[i]]);
            }
            else
            {
                vertexIDs.Add(demandVertices[i - supplyVertices.Count], i + 1);
                IDsToVertex.Add(i + 1, demandVertices[i - supplyVertices.Count]);
                Debug.Log(demandVertices[i - supplyVertices.Count].ToString() + ":" + vertexIDs[demandVertices[i - supplyVertices.Count]]);
            }
        }

    }

    public void solveProblem()
    {
        step0();

        for(int i = 0; i < 100; i++)
        {
            if (partA())
            {
                break;
            }
        }
        int cost = 0;
        Debug.Log("Solution Found!");
        for(int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];
            for(int j = 0; j < demandVertices.Count; j++)
            {
                T dV = demandVertices[j];

                cost = (edgeWeights[sV][dV] * costDict[sV][dV]) + cost;
                Debug.Log(sV.ToString() + ":" + dV.ToString() + ":" + edgeWeights[sV][dV]);
            }
        }

        Debug.Log("Total Cost: " + cost);
    }

    void step0()
    {
        supplyWeights = new Dictionary<T, int>();
        demandWeights = new Dictionary<T, int>();
        //Step 0a
        for (int i = 0; i < supplyVertices.Count; i++)
        {
            int minCost = int.MaxValue;
            T sV = supplyVertices[i];

            foreach (T t in costDict[sV].Keys)
            {
                if (costDict[sV][t] < minCost)
                {
                    minCost = costDict[sV][t];
                }
            }

            supplyWeights.Add(sV, minCost);
            Debug.Log(sV.ToString() + " Weight: " + minCost);
        }

        workedCostDict = new Dictionary<T, Dictionary<T, int>>();

        foreach(T t in costDict.Keys)
        {

            Dictionary<T, int> innerDic = new Dictionary<T, int>();
            foreach(T s in costDict[t].Keys)
            {
                innerDic.Add(s, costDict[t][s]);
            }
            workedCostDict.Add(t, innerDic);

        }



        for (int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];
            int weight = supplyWeights[sV];

            List<T> keys = new List<T>(workedCostDict[sV].Keys);

            foreach (T t in keys)
            {
                workedCostDict[sV][t] -= weight;
            }
        }

        for (int i = 0; i < demandVertices.Count; i++)
        {
            T dV = demandVertices[i];
            int minCost = int.MaxValue;
            foreach (T t in workedCostDict.Keys)
            {
                if (workedCostDict[t][dV] < minCost)
                {
                    minCost = workedCostDict[t][dV];
                }
            }

            demandWeights.Add(dV, minCost); Debug.Log(dV.ToString() + " Weight: " + minCost);
        }

        for (int i = 0; i < demandVertices.Count; i++)
        {
            T dV = demandVertices[i];
            int weight = demandWeights[dV];

            List<T> keys = new List<T>(workedCostDict.Keys);
            foreach (T t in keys)
            {
                workedCostDict[t][dV] -= weight;
            }
        }

        for (int i = 0; i < supplyVertices.Count; i++)
        {
            for (int j = 0; j < demandVertices.Count; j++)
            {
                T sV = supplyVertices[i];
                T dV = demandVertices[j];
                Debug.Log(sV.ToString() + ":" + dV.ToString() + ":=" + workedCostDict[sV][dV]);
            }
        }

        //Step 0b
        for (int i = 0; i < supplyVertices.Count; i++)
        {
            for (int j = 0; j < demandVertices.Count; j++)
            {
                T sV = supplyVertices[i];
                T dV = demandVertices[j];

                if (workedCostDict[sV][dV] == 0)
                {
                    partialGraph[sV][dV] = true;
                    Debug.Log(sV.ToString() + ":" + dV.ToString() + ":=" + "true");
                }


            }
        }
    }

    bool partA()
    {
        List<T> keys = new List<T>(labelled.Keys);
        foreach (T t in keys)
        {
            labelled[t] = -1;
            newlyLabelled[t] = false;
        }

        //Debug.Log("Yet another check");
        if (step1())
        {
            return true;
        }
        else
        {
            for(int i = 0; i < 100; i++)
            {
                if (step2())
                {
                    break;
                }
                //Debug.Log("Passing");
                if (step3())
                {
                    break;
                }
                
            }
        }
        return false;
    }

    bool step1()
    {

        bool check = true;
        for(int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];

            if(supplyAmount[sV] != 0)
            {
                labelled[sV] = 0;
                newlyLabelled[sV] = true;
                check = false;
            }
        }

        return check;
    }

    bool step2()
    { 
        for(int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];

            if(labelled[sV] >= 0)
            {
                for (int j = 0; j < demandVertices.Count; j++)
                {
                    T dV = demandVertices[j];

                    if (partialGraph[sV][dV])
                    {
                        if (labelled[dV] == -1)
                        {
                            labelled[dV] = vertexIDs[sV];

                            if (demandAmount[dV] > 0)
                            {
                                //Debug.Log("Breakthrough at: " + sV.ToString() + ": " + dV.ToString());
                                partB(dV);
                                //Debug.Log("?");
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    bool step3()
    {
        int count = 0;


        for(int i = 0; i < demandVertices.Count; i++)
        {
            T dV = demandVertices[i];

            if(labelled[dV] >= 0)
            {
                for (int j = 0; j < supplyVertices.Count; j++)
                {
                    T sV = supplyVertices[j];

                    if (partialGraph[sV][dV] && edgeWeights[sV][dV] > 0 && labelled[sV] == -1)
                    {
                        labelled[sV] = vertexIDs[dV];
                        count += 1;
                        //Debug.Log("Passing here: " + dV.ToString() + ":" + sV.ToString());
                    }
                }
            }
        }

        if(count == 0)
        {
            //Debug.Log("Trying to go to C");
            partC();
            
            return true;
        }
        return false;
    }

    void partB(T bV)
    {
        step4(bV);
    }

    void step4(T bV)
    {
        List<T> path = new List<T>();
        path.Add(bV);
        T cV = bV;
        int flow = demandAmount[bV];
        bool check = true;
        int count = 0;


        while (check)
        {
            T nV = IDsToVertex[labelled[cV]];
            path.Add(nV);
            //Debug.Log("Path: " + nV.ToString() + ":" +cV.ToString());
            if (count % 2 == 1) // backward flow
            {
                //Debug.Log("Passing here maybe");
                if(flow > edgeWeights[cV][nV])
                {
                    flow = edgeWeights[cV][nV];
                }
            }
            if(labelled[nV] == 0)
            {
                if(flow > supplyAmount[nV])
                {
                    flow = supplyAmount[nV];
                }

                
                check = false;
                step5(path, flow);
            }
            cV = nV;
            count += 1;
            if(count == 100)
            {
                //Debug.LogError("Error");
                check = false;
            }

        }





    }

    void step5(List<T> path, int flow)
    {
        for(int i = 1; i < path.Count; i++)
        {
            if(i%2 == 1)
            {
                T sV = path[i];
                T dV = path[i - 1];

                edgeWeights[sV][dV] += flow;
                //Debug.Log(sV.ToString() + ":" + dV.ToString() + ":" + flow);

            }
            else
            {
                T sV = path[i - 1];
                T dV = path[i];

                edgeWeights[sV][dV] -= flow;
                //Debug.Log(sV.ToString() + ":" + dV.ToString() + ":" + flow);
            }
        }

        supplyAmount[path[path.Count - 1]] -= flow;
        demandAmount[path[0]] -= flow;

    }

    void partC()
    {
        step6();
        
    }

    void step6()
    {
        int delta = int.MaxValue;

        for (int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];

            for (int j = 0; j < demandVertices.Count; j++)
            {
                T dV = demandVertices[j];
                if (labelled[sV] >= 0 && labelled[dV] == -1 && workedCostDict[sV][dV] != 0)
                {
                    if (workedCostDict[sV][dV] < 0)
                    {
                        Debug.LogError("ERROR!");
                    }
                    if (workedCostDict[sV][dV] <= delta)
                    {
                        delta = workedCostDict[sV][dV];
                        //Debug.Log("Delta: " + delta + ":" + sV.ToString() + ":" + dV.ToString());
                    }
                }

            }
        }

        //Debug.Log("Delta: " + delta);
        step7(delta);


    }

    void step7(int delta)
    {
        if(delta <= 0)
        {
            Debug.LogError("Uh oh");
        }

        for(int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];
            if(labelled[sV] >= 0)
            {
                supplyWeights[sV] += delta;
            }
            
            for(int j = 0; j < demandVertices.Count; j++)
            {
                T dV = demandVertices[j];
                if(labelled[sV] >= 0 && labelled[dV] == -1)
                {
                    if(workedCostDict[sV][dV] < 0)
                    {
                        Debug.LogError("ERROR!");
                    }
                    workedCostDict[sV][dV] -= delta;

                }
                else if (labelled[sV] == -1 && labelled[dV] >= 0)
                {
                    workedCostDict[sV][dV] += delta;
                }
            }

        }


        for(int i = 0; i < demandVertices.Count; i++)
        {
            T dV = demandVertices[i];
            if(labelled[dV] >= 0)
            {
                demandWeights[dV] -= delta;
            }
        }

        for(int i = 0; i < supplyVertices.Count; i++)
        {
            T sV = supplyVertices[i];
            for(int j = 0; j < demandVertices.Count; j++)
            {
                T dV = demandVertices[j];

                if(workedCostDict[sV][dV] < 0)
                {
                    Debug.LogError("Another fantastic error");
                }
                if (edgeWeights[sV][dV] < 0)
                {
                    Debug.LogError("Another fantastic error");
                }
                

                if(workedCostDict[sV][dV] == 0)
                {
                    partialGraph[sV][dV] = true;
                }
                else
                {
                    partialGraph[sV][dV] = false;
                }

                if (workedCostDict[sV][dV] > 0 && partialGraph[sV][dV])
                {
                    Debug.LogError("Another fantastic error" + sV.ToString() + ":" + dV.ToString() + workedCostDict[sV][dV]);
                }
            }
        }
    }
}




/*
void partA()
{
    List<T> keys = new List<T>(labelled.Keys);
    foreach (T t in keys)
    {
        labelled[t] = -1;
    }

    if (step1())
    {
        Debug.Log("Minimum solution found!");

        for(int i = 0; i < supplyVertices.Count; i++)
        {
            for(int j = 0; j < demandVertices.Count; j++)
            {
                T sV = supplyVertices[i];
                T dV = demandVertices[j];

                Debug.Log("Edge Weight " + sV.ToString() + ":" + dV.ToString() + ":" + edgeWeights[sV][dV]);
            }
        }


        return;
        //Minimum solution found
    }
    else
    {
        for (int i = 0; i < 100; i++)
        {
            if (step2(i))
            {
                break;
            }
            step3();  

        }
    }


}

bool step1()
{
    int checkCount = 0;
    for (int i = 0; i < supplyVertices.Count; i++)
    {
        if (supplyAmount[supplyVertices[i]] != 0)
        {
            labelled[supplyVertices[i]] = 0;
            checkCount += 1;


        }
        else
        {
            labelled[supplyVertices[i]] = -1;
        }
        Debug.Log("Supply " + supplyVertices[i].ToString() + ":" + supplyAmount[supplyVertices[i]]);
    }
    if (checkCount == 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}

 bool step2(int iteration)
{
    int count = 0;
    for (int i = 0; i < supplyVertices.Count; i++)
    {
        T sV = supplyVertices[i];
        if (labelled[sV] >= 0)
        {
            List<T> dVs = new List<T>(partialGraph[sV].Keys);
            foreach (T dV in dVs)
            {
                if (partialGraph[sV][dV])
                {
                    if (labelled[dV] == -1)
                    {
                        labelled[dV] = vertexIDs[sV];
                        count += 1;

                        if (demandAmount[dV] != 0)
                        {
                            Debug.Log("Breakthrough: " + sV.ToString() + ":" +  dV.ToString()+ ":" + demandAmount[dV]);

                            partB(dV);
                            return true;
                        }
                    }

                }

            }
        }
    }

    if(count == 0)
    {
        Debug.Log("Go to part C1");
        partC();
        return true;
    }
    return false;
}

void step3()
{
    int count = 0;
    for (int i = 0; i < demandVertices.Count; i++)
    {
        T dV = demandVertices[i];
        List<T> keys = new List<T>(partialGraph.Keys);
        for (int j = 0; j < keys.Count; j++)
        {
            if (partialGraph[keys[j]][dV])
            {
                if(labelled[keys[j]]==-1 &&edgeWeights[keys[j]][dV] > 0)
                {
                    labelled[keys[j]] = vertexIDs[dV];
                    //Debug.Log("Passing step 3" + keys[j].ToString() + ":" + dV.ToString());
                    count += 1;
                }
            }
        }
    }
    if(count == 0)
    {
        Debug.Log("Go to part C");
        partC();

    }
}

void partB(T bV)
{

    foreach (T lab in labelled.Keys)
    {
        Debug.Log(lab.ToString() + ":" + labelled[lab]);
    }
    step4(bV);


}

void step4(T bV)
{
    int maxFlow = demandAmount[bV];
    bool check = true;

    T cV = bV;
    List<T> path = new List<T>();
    path.Add(cV);
    int count = 1;



    for(int i = 0; i < 100; i++)
    {
        if (labelled.ContainsKey(cV)&&labelled[cV] != 0)
        {
            T nV = IDsToVertex[labelled[cV]];
            path.Add(nV);

            if (count%2 == 0&&edgeWeights[cV][nV] < maxFlow)
            {
                maxFlow = edgeWeights[cV][nV];
            }
            count += 1;
            cV = nV;
        }
        else
        {
            if(supplyAmount[cV] < maxFlow)
            {
                maxFlow = supplyAmount[cV];
            }
            check = false;
            break;
        }
    }

    for(int i = 0; i < path.Count; i++)
    {
        //Debug.Log("Path:" + path[i].ToString() + maxFlow);
    }

    step5(path, maxFlow);
}

void step5(List<T> path, int flow)
{
    for(int i = 1; i < path.Count; i++)
    {
        if(i%2 == 1)
        {
            T sV = path[i];
            T dV = path[i - 1];
            edgeWeights[sV][dV] += flow;
            //Debug.Log(sV.ToString() + ":" + dV.ToString() + ":" + flow);
        }
        else
        {
            T sV = path[i - 1];
            T dV = path[i];
            edgeWeights[sV][dV] -= flow;
        }
    }

    Debug.Log(path[path.Count - 1]);

    supplyAmount[path[path.Count - 1]] -= flow;
    demandAmount[path[0]] -= flow;

    for(int i = 0; i < supplyVertices.Count; i++)
    {
        T sV = supplyVertices[i];
        //Debug.Log(sV.ToString() + ":" + supplyAmount[sV]);
    }
    for (int i = 0; i < demandVertices.Count; i++)
    {
        T dV = demandVertices[i];
        //Debug.Log(dV.ToString() + ":" + demandAmount[dV]);
    }

    partA();
    return;

}


void partC()
{
    step6();
}

void step6()
{
    int delta = int.MaxValue;


    for(int i = 0; i < supplyVertices.Count; i++)
    {
        T sV = supplyVertices[i];
        if (labelled[sV] >= 0)
        {
            for(int j = 0; j < demandVertices.Count; j++)
            {
                T dV = demandVertices[j];
                if (labelled[dV] == -1)
                {

                    if(workedCostDict[sV][dV] != 0)
                    {
                        if(delta > workedCostDict[sV][dV])
                        {
                            delta = workedCostDict[sV][dV];
                            Debug.Log("Passing step 6");
                        }
                    }


                }
            }
        }
    }

    Debug.Log("Delta: " + delta);
    step7(delta);
}
void step7(int delta)
{


    //A & C & D
    for(int i = 0; i < supplyVertices.Count; i++)
    {
        T sV = supplyVertices[i];

        if (labelled[sV] >=0)
        {
            supplyWeights[sV] += delta;
        }
        for(int j = 0; j < demandVertices.Count; j++)
        {
            T dV = demandVertices[j];
            if(labelled[sV] >=0 && labelled[dV] < 0)
            {
                workedCostDict[sV][dV] -= delta;
            }
            if(labelled[sV] < 0 && labelled[dV] >=0)
            {
                workedCostDict[sV][dV] += delta;
            }

        }

    }

    //B
    for(int i = 0; i < demandVertices.Count; i++)
    {
        T dV = demandVertices[i];
        if (labelled[dV] >= 0)
        {
            demandWeights[dV] -= delta;
        }
    }
    //E

    for(int i = 0; i < supplyVertices.Count; i++)
    {

        for(int j = 0; j < demandVertices.Count; j++)
        {
            T sV = supplyVertices[i];
            T dV = demandVertices[j];

            if(workedCostDict[sV][dV] == 0)
            {
                partialGraph[sV][dV] = true;
            }
            else
            {
                partialGraph[sV][dV] = false;
            }
            if (workedCostDict[sV][dV] < 0)
            {
                Debug.LogError(sV.ToString() + ":" + dV.ToString() + ":" + workedCostDict[sV][dV]);
            }

        }
    }

    Debug.Log("Back to A");
    //partA();

}

}
*/
