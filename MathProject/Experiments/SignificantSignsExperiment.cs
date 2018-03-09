﻿using MathNet.Numerics.LinearAlgebra;
using MathProject.Entities;
using MathProject.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathProject.Experiments
{
    public class SignificantSignsExperiment
    {
        NgonDatabase database = new NgonDatabase();
        static Dictionary<SignMatrix, bool> pluckerMatrices = new Dictionary<SignMatrix, bool>();

        public SignificantSignsExperiment(int dimensions)
        {
            this.database = new NgonDatabase(dimensions);
        }

        public void findSignificantSigns()
        {
            foreach (SignMatrix matrix in database.PluckerSignMatrices)
            {
                pluckerMatrices.Add(matrix, matrix.Ngons.Count(n => n.Type == NgonType.Convex) != 0);
            }
            List<Matrix<double>> masks = createMasks();
            List<Matrix<double>> result = checkMasks(masks);
            count(result);

            List<Matrix<double>> masksl2 = createMasks(result);
            result = checkMasks(masksl2);
            count(result);

            List<Matrix<double>> masksl3 = createMasks(checkMasks(result));
            result = checkMasks(masksl3);
            count(result);

            foreach (var mask in result)
                Console.WriteLine(mask);
        } 
        private void count(List<Matrix<double>> result)
        {            
            for (int i = 1; i < 6; i++)
            {
                double s = result[0].ColumnSums().Sum();
                Console.WriteLine(i + ": " + result.Count(m => m.ColumnSums().Sum() == 25-i));                
            }
            Console.WriteLine();
        }

        private List<Matrix<double>> checkMasks(List<Matrix<double>> masks)
        {
            List<Matrix<double>> goodMasks = new List<Matrix<double>>();
            foreach (var mask in masks)
            {
                bool ambiguity = false;
                foreach (SignMatrix matrix1 in pluckerMatrices.Keys)
                {
                    if (ambiguity) break;
                    bool matrix1Convex = pluckerMatrices[matrix1];
                    if (matrix1Convex)
                    {
                        //var similarMatricies = pluckerMatrices.Keys.Where(m2 => matricesEqual(matrix1, m2, mask.AsColumnArrays()));
                        if (pluckerMatrices.Keys.Any(m2 => !pluckerMatrices[m2] && matrix1.EqualsWithMask(m2, mask)))
                            ambiguity = true;
                    }
                }
                if (!ambiguity)
                {
                    goodMasks.Add(mask);                    
                }
            }
            return goodMasks;
        }

        private List<Matrix<double>> createMasks(List<Matrix<double>> masks)
        {
            List<Matrix<double>> newMasks = new List<Matrix<double>>();
            int dimensions = pluckerMatrices.Keys.First().columnVectors[0].Length;
            foreach (Matrix<double> mask in masks)
                for (int ni = 1; ni < dimensions; ni++)
                {
                    for (int nj = 0; nj < ni; nj++)
                    {
                        var newMask = Matrix<double>.Build.DenseOfMatrix(mask);
                        newMask[nj,ni] = 0;
                        if(!newMasks.Contains(newMask))
                            newMasks.Add(newMask);
                    }
                }
            return newMasks;
        }
        private List<Matrix<double>> createMasks()
        {
            int dimensions = pluckerMatrices.Keys.First().columnVectors[0].Length;
            Matrix<double> mask = Matrix<double>.Build.Dense(dimensions, dimensions, 1);
            return  createMasks(new List<Matrix<double>>() { mask });
        }

        private string printMask(int[][] mask)
        {
            string result = "";
            for (int i = 0; i < mask[0].Count(); i++)
            {
                result += "| ";
                for (int j = 0; j < mask.Count(); j++)
                {
                    if (j < i)
                    {
                        result += "  ";
                        continue;
                    }
                    if (mask[j][i] == -1) result += "- ";
                    if (mask[j][i] == 1) result += "+ ";
                    if (mask[j][i] == 0) result += "0 ";
                }
                result += "|\n";
            }
            return result;
        }
    }
}