using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Accord.Statistics.Distributions.Univariate;
using Accord.Math.Decompositions;
using MathNet.Numerics.LinearAlgebra;
using MathProject.Entities;
using System.IO;
using MathProject.Tools;

namespace MathProject
{
    public class Program
    {
        static void Main(string[] args)
        {
            PluckerSignMatrix p = new PluckerSignMatrix(generateRandomNgon(5));
            Console.WriteLine(p);
            Console.Read();
        }

        private static void NgonEdgePermutationExperiment()
        {
            Console.WriteLine("How many times to repeat the experiment?");
            long outerSampleSize = Int64.Parse(Console.ReadLine());
            Console.WriteLine("How many dimensions are we working with?");
            int n = Int32.Parse(Console.ReadLine());

            Console.WriteLine("How big is the sample size?");
            long sampleSize = Int64.Parse(Console.ReadLine());
            List<double[]> results = new List<double[]>();
            for (long i = sampleSize; i > 0; i--)
            {
                if (i % (sampleSize / 10) == 0) Console.WriteLine(i / (sampleSize / 100) + "%");
                Ngon ngon = generateRandomNgon(n);

                var permutations = (new NgonEdgePermutations(ngon)).edgePermutations();
                int convex = permutations.Count(m => m.Type == NgonType.Convex);
                int reflex = permutations.Count(m => m.Type == NgonType.Reflex);
                int self_intersecting = permutations.Count(m => m.Type == NgonType.Self_Intersecting);

                double[] entry = results.Find(e => e[0] == convex && e[1] == reflex && e[2] == self_intersecting);
                if (entry != null) entry[3]++;
                else results.Add(new double[] { convex, reflex, self_intersecting, 1 });
            }
            foreach (double[] entry in results)
            {
                writeCSV(entry);
            }
            Console.WriteLine("DONE");
            Console.Read();
        }

        private static void writeCSV(double[] toWrite)
        {
            StreamWriter writer = new StreamWriter((new FileStream("experiment2.csv", FileMode.Append)));
            for(int i = 0;i < toWrite.Length-1;i++)
            {
                writer.Write(toWrite[i].ToString() + ';');
            }
            writer.WriteLine(toWrite[toWrite.Length - 1]);
            writer.Close();
        }


        public static Ngon generateRandomNgon(int n)
        {
            double[] vector1 = generateVector(n);
            double[] vector2 = generateVector(n);

            double[][] orthonormal = GramSchmidt(new double[][] { vector1, vector2 });
            double[][] edgeVectors = squareEntries(orthonormal);

            return new Ngon(edgeVectors,orthonormal);
        }

        public static double[] generateVector(int n)
        {
            NormalDistribution randomizer = new NormalDistribution(0, 1);
            double[] vector = new double[n];

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = randomizer.Generate();
            }
            return vector;
        }

        public static double[][] GramSchmidt(double[][] vectors)
        {
            Matrix<double> matrix = CreateMatrix.DenseOfColumnArrays(vectors);
            var result = matrix.GramSchmidt();
            return result.Q.ToColumnArrays();
        }

        public static double[][] squareEntries(double[][] complexNumbers)
        {
            double[][] result =new double[complexNumbers[0].Length][];
            for(int j=0;j<complexNumbers[0].Length;j++)
            {
                double real = Math.Pow(complexNumbers[0][j], 2)- Math.Pow(complexNumbers[1][j], 2); //Math.Pow(row[0],2)
                double imaginary = 2 * complexNumbers[0][j] * complexNumbers[1][j]; //2*u_j*v_j
                result[j] = new double[] { real, imaginary };
            }
            return result;
        }
    }

   

    
}

