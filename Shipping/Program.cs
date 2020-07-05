using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Shipping
{
    class Program
    {
        const int INF = 1_000_000_000;
     
        private class Graph
        {
            public int[][] EdgesWeight { get; }
            public int VertexCount { get; }

            public Graph(int[][] edgesWeight, int vertexCount)
            {
                this.EdgesWeight = edgesWeight;
                this.VertexCount = vertexCount;
            }
        }

        static private Graph CreateGraph(string path)
        {
            try
            {
                int[][] temp;
                int numberOfVertexes;

                using (StreamReader streamReader = new StreamReader(path))
                {
                    numberOfVertexes = int.Parse(streamReader.ReadLine());
                    temp = new int[numberOfVertexes][];

                    for (int i = 0; i < numberOfVertexes; i++)
                    {
                        temp[i] = new int[numberOfVertexes];
                        for (int j = 0; j < numberOfVertexes; j++)
                        {
                            temp[i][j] = INF;
                        }
                    }

                    while (!streamReader.EndOfStream) //считывание...
                    {
                        int[] edgeInfo = streamReader
                            .ReadLine()
                            .Split('\t')
                            .Select(int.Parse)
                            .ToArray();

                        int VertexOut = edgeInfo[0] - 1;
                        int VertexIn = edgeInfo[1] - 1;
                        temp[VertexOut][VertexIn] = edgeInfo[2];
                    } //...путей
                }

                return new Graph(temp, numberOfVertexes);
            }
            catch
            {
                throw new Exception("Неправильный формат файла с данными.");
            }
            
        }

        static private Graph FloydWarshall(Graph costGraph)
        {
            int n = costGraph.VertexCount;
            int[][] d = costGraph.EdgesWeight;
            
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (d[i][j] > d[i][k] + d[k][j])
                            d[i][j] = d[i][k] + d[k][j];

            return new Graph(d, n);
        }

        static void Main(string[] args)
        {

            string path = "warehouse_info.txt"; //путь до информации о складах
            Graph costGraph;

            try
            {
                costGraph = CreateGraph(path);
                costGraph = FloydWarshall(costGraph);

                Console.WriteLine("Откуда производится перевозка:");
                int a = int.Parse(Console.ReadLine()) - 1;

                Console.WriteLine("Куда производится перевозка:");
                int b = int.Parse(Console.ReadLine()) - 1;

                if (a < 0 || a > 19 || b < 0 || b > 19)
                    throw new Exception("Склад(-ы) не существует(-ют).");

                if (a == b)
                    throw new Exception("Ошибка! Такие перевозки не допускаются.");

                if (costGraph.EdgesWeight[a][b] == INF)
                    throw new Exception("Невозможно совершить перевозку. Пути не существует.");

                Console.WriteLine("Минимальная стоимость перевозки:\n"
                            + costGraph.EdgesWeight[a][b]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.ReadLine();
        }
    }
}
