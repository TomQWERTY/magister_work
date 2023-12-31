﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CourseWork
{
    public class Controller
    {
        Model model;
        ViewForm view;
        public Controller()
        {
            ApplicationConfiguration.Initialize();
            view = new ViewForm(this);
            model = new Model(view);
        }

        public void NewModel()
        {
            model = new Model(view);
        }

        public void Run()
        {
            Application.Run(view);
        }

        public void AddPlaceElement()
        {
            model.AddPlaceElement();
        }

        public void AddTransitionElement()
        {
            model.AddTransitionElement();
        }

        public bool TransitionsPresent
        {
            get
            {
                return model.TransitionsPresent;
            }
        }

        public void AddConection(bool placeToTrans, int ind1, int ind2, int val)
        {
            model.AddConection(placeToTrans, ind1, ind2, val);
        }

        public void AddTokens(int ind, int num)
        {
            model.AddTokens(ind, num);
        }

        public bool TryDeleteLastPlace()
        {
            return model.TryDeleteLastPlace();
        }

        public bool TryDeleteLastTransition()
        {
            return model.TryDeleteLastTransition();
        }

        public void ImitationStep()
        {
            List<int> availableIndexes = new List<int>();
            //check all transition elements
            for (int j = 0; j < model.matrixW[0].Count; j++)
            {
                bool canActivate = true;
                for (int i = 0; i < model.matrixW.Count; i++)
                {
                    if (model.matrixW[i][j] < 0 && model.marking[i] < model.matrixW[i][j] * -1)
                    {
                        canActivate = false;
                        break;
                    }
                }
                if (canActivate)
                {
                    availableIndexes.Add(j);
                }
            }
            if (availableIndexes.Count > 0)
            {
                Random rnd = new Random();
                int activateIndex = availableIndexes[rnd.Next(0, availableIndexes.Count)];
                ActivateTransitionElement(activateIndex);
                view.ClearTransitionMarks();
                view.MarkTransition(activateIndex, false);
                //to mark new next transition
                for (int k = 0; k < model.matrixW[0].Count; k++)
                {
                    bool canActivate2 = true;
                    for (int i = 0; i < model.matrixW.Count; i++)
                    {
                        if (model.matrixW[i][k] < 0 && model.marking[i] < model.matrixW[i][k] * -1)
                        {
                            canActivate2 = false;
                            break;
                        }
                    }
                    if (canActivate2)
                    {
                        view.MarkTransition(k, true);
                    }
                }
            }

        }

        private void ActivateTransitionElement(int ind)
        {
            for (int i = 0; i < model.matrixW.Count; i++)
            {
                int newVal = model.marking[i] + model.matrixW[i][ind];
                model.marking[i] = newVal;
                view.ModifyTokens(i, newVal, true);
            }
        }

        public void SaveData(string fileName, ViewSaveData viewSaveData)
        {
            model.Save(fileName, viewSaveData);
        }

        public void LoadData(string fileName, out ViewSaveData viewSaveData)
        {
            model.Load(fileName, out viewSaveData);
        }

        public void UpdateView()
        {
            model.UpdateView();
        }

        public void UpdateTokens() { model.UpdateTokens(); }

        public void Analyze(bool getTime, int tries)
        {
            AnalysisType at = view.GetAnalysisMethod();
            bool[] infoForView = new bool[6];
            List<int> notCoveredIndsT = new List<int>();
            List<int> notCoveredIndsP = new List<int>();
            if (at != AnalysisType.None)
            {
                int[,] w = new int[model.matrixW.Count, model.matrixW[0].Count];
                for (int i = 0; i < w.GetLength(0); i++)
                {
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        w[i, j] = model.matrixW[i][j];
                    }
                }
                long sum = 0;
                int[,] tInvs = new int[1, 1];
                for (int tryInd = 0; tryInd < tries; tryInd++)
                {
                    DateTime dt1 = DateTime.Now;
                    switch (at)
                    {
                        case AnalysisType.TSS: tInvs = CheckInvariantsTSS(w); break;
                        case AnalysisType.Alaivan: tInvs = CheckInvariantsAlaivan(w); break;
                        case AnalysisType.Farkas: tInvs = CheckInvariantsFar(w); break;
                        case AnalysisType.TSSMod: tInvs = CheckInvariantsTSSMod(w); break;
                        case AnalysisType.TSSOpt: tInvs = CheckInvariantsTSSOpt(w); break;
                        case AnalysisType.AlaivanOpt: tInvs = CheckInvariantsAlaivanOpt(w); break;
                    }
                    ReduceCoefficients(tInvs);
                    //check dynamic properties
                    bool[] covered = new bool[tInvs.GetLength(1)];
                    for (int i = 0; i < tInvs.GetLength(1); i++)
                    {
                        for (int j = 0; j < tInvs.GetLength(0); j++)
                        {
                            if (tInvs[j, i] != 0)
                            {
                                covered[i] = true;
                                break;
                            }
                        }
                        if (!covered[i]) notCoveredIndsT.Add(i);
                    }

                    if (notCoveredIndsT.Count == 0)
                    {
                        infoForView[0] = true;
                        infoForView[1] = true;
                        infoForView[2] = true;
                    }
                    sum += (DateTime.Now - dt1).Ticks;
                }
                string tInvsTime = (sum * 1.0 / tries / TimeSpan.TicksPerMillisecond) + "";
                if (tInvs.Length == 0 || tInvs.GetLength(0) > 100 || at == AnalysisType.TSSMod)
                {
                    tInvs = new int[1, tInvs.GetLength(1)];
                    for (int i = 0; i < tInvs.GetLength(1); i++)
                    {
                        tInvs[0, i] = 0;
                    }
                }
                
                //
                w = new int[model.matrixW[0].Count, model.matrixW.Count];
                for (int i = 0; i < w.GetLength(0); i++)
                {
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        w[i, j] = model.matrixW[j][i];
                    }
                }
                int[,] pInvs = new int[1, 1];
                sum = 0;
                for (int tryInd = 0; tryInd < tries; tryInd++)
                {
                    DateTime dt1 = DateTime.Now;
                    switch (at)
                    {
                        case AnalysisType.TSS: pInvs = CheckInvariantsTSS(w); break;
                        case AnalysisType.Alaivan: pInvs = CheckInvariantsAlaivan(w); break;
                        case AnalysisType.Farkas: pInvs = CheckInvariantsFar(w); break;
                        case AnalysisType.TSSMod: pInvs = CheckInvariantsTSSMod(w); break;
                        case AnalysisType.TSSOpt: pInvs = CheckInvariantsTSSOpt(w); break;
                        case AnalysisType.AlaivanOpt: pInvs = CheckInvariantsAlaivanOpt(w); break;
                    }
                    ReduceCoefficients(pInvs);
                    //check dynamic properties
                    
                    bool[] covered = new bool[pInvs.GetLength(1)];
                    for (int i = 0; i < pInvs.GetLength(1); i++)
                    {
                        for (int j = 0; j < pInvs.GetLength(0); j++)
                        {
                            if (pInvs[j, i] != 0)
                            {
                                covered[i] = true;
                                break;
                            }
                        }
                        if (!covered[i]) notCoveredIndsP.Add(i);
                    }
                    if (notCoveredIndsP.Count == 0)
                    {
                        infoForView[3] = true;
                        infoForView[4] = true;
                    }
                    sum += (DateTime.Now - dt1).Ticks;
                }
                string pInvsTime = (sum * 1.0 / tries / TimeSpan.TicksPerMillisecond) + "";
                if (pInvs.Length == 0 || pInvs.GetLength(0) > 100 || at == AnalysisType.TSSMod)
                {
                    pInvs = new int[1, pInvs.GetLength(1)];
                    for (int i = 0; i < pInvs.GetLength(1); i++)
                    {
                        pInvs[0, i] = 0;
                    }
                }
                
                double[,] matrixDataForRank = new double[model.matrixW.Count, model.matrixW[0].Count];
                for (int i = 0; i < matrixDataForRank.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixDataForRank.GetLength(1); j++) matrixDataForRank[i, j] = model.matrixW[i][j];
                }
                var matrixForRank = Matrix<double>.Build.DenseOfArray(matrixDataForRank);
                int rank = matrixForRank.Rank();
                if (rank >= Math.Min(model.matrixW.Count, model.matrixW[0].Count))
                {
                    infoForView[5] = true;
                }
                if (getTime)
                    view.ShowResults(infoForView, rank, tInvs, pInvs, notCoveredIndsT, notCoveredIndsP, tInvsTime, pInvsTime);
                else view.ShowResults(infoForView, rank, tInvs, pInvs, notCoveredIndsT, notCoveredIndsP);
            }
        }

        private int[,] ReduceCoefficients(int[,] solutions)
        {
            int[] allParVals = new int[solutions.GetLength(0) * (solutions.GetLength(1) - 1)];
            for (int v = 0, i = 0; v < solutions.GetLength(0); v++)
            {
                for (int p = 1; p < solutions.GetLength(1); p++, i++)
                {
                    allParVals[i] = solutions[v, p];
                }
            }
            int gcdArr = GCDArray(allParVals);
            for (int v = 0; v < solutions.GetLength(0); v++)
            {
                for (int p = 0; p < solutions.GetLength(1); p++)
                {
                    if (gcdArr != 0) solutions[v, p] /= gcdArr;
                }
            }
            return solutions;
        }

        private int[,] CheckInvariantsFar(int[,] wInv)
        {
            //inverting a matrix
            int[,] w = new int[wInv.GetLength(1), wInv.GetLength(0)];
            for (int i = 0; i < w.GetLength(0); i++)
            {
                for (int j = 0; j < w.GetLength(1); j++)
                {
                    w[i, j] = wInv[j, i];
                }
            }
            int rowCountInit = w.GetLength(0);
            int colCount = w.GetLength(1) + w.GetLength(0);
            List<int[]> c = new List<int[]>();
            //creating augmented matrix
            for (int rowI = 0; rowI < rowCountInit; rowI++)
            {
                c.Add(new int[colCount]);
                c[rowI][rowI] = 1;//invariance
                for (int colI = rowCountInit; colI < colCount; colI++)//incidence
                {
                    c[rowI][colI] = w[rowI, colI - rowCountInit];
                }
            }
            for (int colI = rowCountInit; colI < colCount; colI++)//check incidence matrix columns
            {
                //count pos and neg elements indexes
                List<int> posInds = new List<int>(), negInds = new List<int>();
                for (int rowI = 0; rowI < c.Count; rowI++)
                {
                    if (c[rowI][colI] < 0) negInds.Add(rowI);
                    else if (c[rowI][colI] > 0) posInds.Add(rowI);
                }
                int newRowCount = 0;
                //for each pair add its sum
                for (int pI = 0; pI < posInds.Count; pI++)
                {
                    int j = posInds[pI];
                    for (int nI = 0; nI < negInds.Count; nI++)
                    {
                        int i = negInds[nI];
                        int gcd = GCD(Math.Abs(c[j][colI]), Math.Abs(c[i][colI]));
                        int iMult = Math.Abs(c[j][colI]) / gcd;
                        int jMult = Math.Abs(c[i][colI]) / gcd;
                        int[] newRow = new int[colCount];
                        for (int k = 0; k < colCount; k++)
                        {
                            newRow[k] = c[j][k] * jMult + c[i][k] * iMult;
                        }
                        c.Add(newRow);
                        newRowCount++;
                    }
                }
                //remove originals
                List<int> indsToRemove = posInds;
                indsToRemove.AddRange(negInds);
                indsToRemove.Sort();
                for (int i = indsToRemove.Count - 1; i >= 0; i--) c.RemoveAt(indsToRemove[i]);
                //checking
                for (int r = c.Count - newRowCount; r < c.Count; r++)
                {
                    int[] comp1 = c[r];
                    bool coversSome = false;
                    for (int cp = 0; cp < c.Count; cp++)
                    {
                        if (cp == r) continue;
                        int[] comp2 = c[cp];
                        bool covers = true;
                        for (int col = 0; col < colCount; col++)
                        {
                            if (comp1[col] == 0 && comp2[col] != 0)
                            {
                                covers = false;
                                break;
                            }
                        }
                        if (covers)
                        {
                            coversSome = true;
                            break;
                        }
                    }
                    if (coversSome)
                    {
                        c.RemoveAt(r);
                        r--;
                    }
                }
            }
            int[,] solutions = new int[c.Count, rowCountInit];
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                for (int j = 0; j < solutions.GetLength(1); j++)
                {
                    solutions[i, j] = c[i][j];
                }
            }
            return solutions;
        }

        private int[,] CheckInvariantsAlaivan(int[,] c)
        {
			List<int[]> eM = new List<int[]>();//extended matrix (first index is column)
			int cRowCount = c.GetLength(0);
			int extRowCount = cRowCount + c.GetLength(1);
			for (int colI = 0; colI < c.GetLength(1); colI++)
			{
				eM.Add(new int[extRowCount]);
				for (int rowI = 0; rowI < cRowCount; rowI++)
				{
					eM[colI][rowI] = c[rowI, colI];
				}
				eM[colI][cRowCount + colI] = 1;
			}
            List<int> nonZeroRowsInds = new List<int>();
            for (int rowI = 0; rowI < cRowCount; rowI++)
            {
                int nonZeroCount = 0;
                for (int colI = 0; colI < eM.Count; colI++) if (eM[colI][rowI] != 0) nonZeroCount++;
                if (nonZeroCount > 0) nonZeroRowsInds.Add(rowI);
            }
			//phase 1
            while (nonZeroRowsInds.Count > 0 && eM.Count > 0)
            {
                int h = -1;
                List<int> posInds = new List<int>();
                List<int> negInds = new List<int>();
                int[] methodsIndexes = new int[3] { -1, -1, -1 };
                for (int rowII = 0; rowII < nonZeroRowsInds.Count; rowII++)
                {
                    int rowI = nonZeroRowsInds[rowII];
                    int posCount = 0, negCount = 0;
                    for (int colI = 0; colI < eM.Count; colI++)
                    {
                        if (eM[colI][rowI] > 0) posCount++;
                        else if (eM[colI][rowI] < 0) negCount++;
                    }
                    if (posCount + negCount == 1)
                    {
                        methodsIndexes[0] = rowI;
                        break;
                    }
                    else if ((posCount == 1 || negCount == 1) && methodsIndexes[1] == -1) methodsIndexes[1] = rowI;
                    else if (methodsIndexes[2] == -1) methodsIndexes[2] = rowI;
                }
                if (methodsIndexes[0] != -1) h = methodsIndexes[0];
                else if (methodsIndexes[1] != -1) h = methodsIndexes[1];
                else h = methodsIndexes[2];
                nonZeroRowsInds.Remove(h);//this row will be zero
                for (int colI = 0; colI < eM.Count; colI++)
                {
                    if (eM[colI][h] > 0)
                    {
                        posInds.Add(colI);
                    }
                    else if (eM[colI][h] < 0)
                    {
                        negInds.Add(colI);
                    }
                }
                if (methodsIndexes[0] != -1)//[1.1]
                {
                    foreach (int ind in posInds) eM.RemoveAt(ind);
                    foreach (int ind in negInds) eM.RemoveAt(ind);
                }
                else if (methodsIndexes[1] != -1)//[1.1.b]
                {
                    int k = 0;
                    List<int> otherCols = null;
                    if (posInds.Count == 1)
                    {
                        k = posInds[0];
                        otherCols = negInds;
                    }
                    else if (negInds.Count == 1)
                    {
                        k = negInds[0];
                        otherCols = posInds;
                    }
                    else continue;
                    for (int ji = 0; ji < otherCols.Count; ji++)//[1.1.b.1]
                    {
                        int j = otherCols[ji];
                        int jMult = Math.Abs(eM[j][h]);
                        int kMult = Math.Abs(eM[k][h]);
                        for (int jRow = 0; jRow < extRowCount; jRow++)
                        {
                            eM[j][jRow] = eM[j][jRow] * kMult + eM[k][jRow] * jMult;
                        }
                    }
                    eM.RemoveAt(k);
                }
                else//[1.1.b.2]
                {
                    List<int> nonzeroInds = new List<int>(posInds.Concat(negInds));
                    int k = nonzeroInds[0];
                    for (int ji = 1; ji < nonzeroInds.Count; ji++)
                    {
                        int j = nonzeroInds[ji];
                        int alpha = Math.Abs(eM[j][h]);//if signs are different
                        int beta = Math.Abs(eM[k][h]);//
                        if (eM[j][h] * 1.0 / eM[k][h] > 0)//if signs are the same
                        {
                            alpha *= -1;
                        }
                        for (int jRow = 0;jRow < extRowCount; jRow++)
                        {
                            eM[j][jRow] = eM[j][jRow] * beta + eM[k][jRow] * alpha;
                        }
                    }
                    eM.RemoveAt(k);
                }
                for (int rowI = 0; rowI < nonZeroRowsInds.Count; rowI++)//maybe after the deletion some rows are now zero
                {
                    bool stillNonZero = false;
                    for (int colI = 0; colI < eM.Count; colI++)
                    {
                        if (eM[colI][nonZeroRowsInds[rowI]] != 0)
                        {
                            stillNonZero = true;
                            break;
                        }
                    }
                    if (!stillNonZero)
                    {
                        nonZeroRowsInds.RemoveAt(rowI);
                        rowI--;
                    }
                }
            }
            //getting rid of matrix C
            List<int[]> B = new List<int[]>();
            int bRowCount = extRowCount - cRowCount;
            for (int colI = 0; colI < eM.Count; colI++)
            {
                B.Add(new int[bRowCount]);
                for (int rowI = 0; rowI < bRowCount; rowI++)
                {
                    B[colI][rowI] = eM[colI][cRowCount + rowI];
                }
            }
            //phase 2
            for (int h = 0; h < bRowCount; h++)
            {
                List<int> posInds = new List<int>();
                List<int> negInds = new List<int>();
                for (int colI = 0; colI < B.Count; colI++)
                {
                    if (B[colI][h] > 0)
                    {
                        posInds.Add(colI);
                    }
                    else if (B[colI][h] < 0)
                    {
                        negInds.Add(colI);
                    }
                }
                if (negInds.Count > 0)
                {
                    if (posInds.Count > 0)
                    {
                        for (int j = 0; j < posInds.Count; j++)//for each pair
                        {
                            for (int k = 0; k < negInds.Count; k++)
                            {
                                //to match coefficients
                                int jAbs = Math.Abs(B[posInds[j]][h]);
                                int kAbs = Math.Abs(B[negInds[k]][h]);
                                int gcd = GCD(jAbs, kAbs);
                                int finalCoef = jAbs / gcd * kAbs;
                                int jMult = finalCoef / jAbs;
                                int kMult = finalCoef / kAbs;
                                if (posInds[j] * jMult == negInds[k] * kMult) kMult *= -1;
                                int[] newColumn = new int[bRowCount];
                                for (int rowI = 0; rowI < newColumn.Length; rowI++)
                                {
                                    newColumn[rowI] = B[posInds[j]][rowI] * jMult + B[negInds[k]][rowI] * kMult;
                                }
                                int columnGCD = Math.Abs(GCDArray(newColumn));
                                for (int i = 0; i < newColumn.Length; i++) newColumn[i] /= columnGCD;
                                B.Add(newColumn);
                            }
                        }
                    }
                    for (int negIndI = negInds.Count - 1; negIndI >= 0; negIndI--)
                    {
                        B.RemoveAt(negInds[negIndI]);
                    }
                }
            }
            //support calculation
            for (int chk = 0; chk < B.Count; chk++)
            {
                bool hasMinSupp = true;
                for (int comp = 0; comp < B.Count; comp++)
                {
                    bool compFine = false;
                    for (int rowI = 0; rowI < bRowCount; rowI++)
                    {
                        if (B[chk][rowI] == 0 && B[comp][rowI] != 0)
                        {
                            compFine = true;
                            break;
                        }
                    }
                    if (!compFine)
                    {
                        for (int rowI = 0; rowI < bRowCount; rowI++)
                        {
                            if (B[chk][rowI] != 0 && B[comp][rowI] == 0)
                            {
                                hasMinSupp = false;
                                break;
                            }
                        }
                        if (!hasMinSupp) break;
                    }
                }
                if (!hasMinSupp)
                {
                    B.RemoveAt(chk);
                    chk--;
                }
            }
            //converting
            int[,] solutions = new int[B.Count, bRowCount];
            for (int rowI = 0; rowI < solutions.GetLength(0); rowI++)
            {
                for (int colI = 0; colI < solutions.GetLength(1); colI++)
                {
                    solutions[rowI, colI] = B[rowI][colI];
                }
            }
            return solutions;
        }

        private int[,] CheckInvariantsAlaivanOpt(int[,] c)
        {
            List<int[]> eM = new List<int[]>();//extended matrix (first index is column)
            int cRowCount = c.GetLength(0);
            int extRowCount = cRowCount + c.GetLength(1);
            for (int colI = 0; colI < c.GetLength(1); colI++)
            {
                eM.Add(new int[extRowCount]);
                for (int rowI = 0; rowI < cRowCount; rowI++)
                {
                    eM[colI][rowI] = c[rowI, colI];
                }
                eM[colI][cRowCount + colI] = 1;
            }
            List<int> nonZeroRowsInds = new List<int>();
            for (int rowI = 0; rowI < cRowCount; rowI++)
            {
                int nonZeroCount = 0;
                for (int colI = 0; colI < eM.Count; colI++) if (eM[colI][rowI] != 0) nonZeroCount++;
                if (nonZeroCount > 0) nonZeroRowsInds.Add(rowI);
            }
            //phase 1
            while (nonZeroRowsInds.Count > 0 && eM.Count > 0)
            {
                int h = -1;
                List<int> posInds = new List<int>();
                List<int> negInds = new List<int>();
                int[] methodsIndexes = new int[3] { -1, -1, -1 };
                for (int rowII = 0; rowII < nonZeroRowsInds.Count; rowII++)
                {
                    int rowI = nonZeroRowsInds[rowII];
                    int posCount = 0, negCount = 0;
                    for (int colI = 0; colI < eM.Count; colI++)
                    {
                        if (eM[colI][rowI] > 0) posCount++;
                        else if (eM[colI][rowI] < 0) negCount++;
                    }
                    if (posCount + negCount == 1)
                    {
                        methodsIndexes[0] = rowI;
                        break;
                    }
                    else if ((posCount == 1 || negCount == 1) && methodsIndexes[1] == -1) methodsIndexes[1] = rowI;
                    else if (methodsIndexes[2] == -1) methodsIndexes[2] = rowI;
                }
                if (methodsIndexes[0] != -1) h = methodsIndexes[0];
                else if (methodsIndexes[1] != -1) h = methodsIndexes[1];
                else h = methodsIndexes[2];
                nonZeroRowsInds.Remove(h);//this row will be zero
                for (int colI = 0; colI < eM.Count; colI++)
                {
                    if (eM[colI][h] > 0)
                    {
                        posInds.Add(colI);
                    }
                    else if (eM[colI][h] < 0)
                    {
                        negInds.Add(colI);
                    }
                }
                if (methodsIndexes[0] != -1)//[1.1]
                {
                    foreach (int ind in posInds) eM.RemoveAt(ind);
                    foreach (int ind in negInds) eM.RemoveAt(ind);
                }
                else if (methodsIndexes[1] != -1)//[1.1.b]
                {
                    int k = 0;
                    List<int> otherCols = null;
                    if (posInds.Count == 1)
                    {
                        k = posInds[0];
                        otherCols = negInds;
                    }
                    else if (negInds.Count == 1)
                    {
                        k = negInds[0];
                        otherCols = posInds;
                    }
                    else continue;
                    for (int ji = 0; ji < otherCols.Count; ji++)//[1.1.b.1]
                    {
                        int j = otherCols[ji];
                        int jMult = Math.Abs(eM[j][h]);
                        int kMult = Math.Abs(eM[k][h]);
                        for (int jRow = 0; jRow < extRowCount; jRow++)
                        {
                            eM[j][jRow] = eM[j][jRow] * kMult + eM[k][jRow] * jMult;
                        }
                    }
                    eM.RemoveAt(k);
                }
                else//[1.1.b.2]
                {
                    List<int> nonzeroInds = new List<int>(posInds.Concat(negInds));
                    int k = nonzeroInds[0];
                    for (int ji = 1; ji < nonzeroInds.Count; ji++)
                    {
                        int j = nonzeroInds[ji];
                        int alpha = Math.Abs(eM[j][h]);//if signs are different
                        int beta = Math.Abs(eM[k][h]);//
                        if (eM[j][h] * 1.0 / eM[k][h] > 0)//if signs are the same
                        {
                            alpha *= -1;
                        }
                        for (int jRow = 0; jRow < extRowCount; jRow++)
                        {
                            eM[j][jRow] = eM[j][jRow] * beta + eM[k][jRow] * alpha;
                        }
                    }
                    eM.RemoveAt(k);
                }
                for (int rowI = 0; rowI < nonZeroRowsInds.Count; rowI++)//maybe after the deletion some rows are now zero
                {
                    bool stillNonZero = false;
                    for (int colI = 0; colI < eM.Count; colI++)
                    {
                        if (eM[colI][nonZeroRowsInds[rowI]] != 0)
                        {
                            stillNonZero = true;
                            break;
                        }
                    }
                    if (!stillNonZero)
                    {
                        nonZeroRowsInds.RemoveAt(rowI);
                        rowI--;
                    }
                }
            }
            //getting rid of matrix C
            List<int[]> B = new List<int[]>();
            int bRowCount = extRowCount - cRowCount;
            for (int colI = 0; colI < eM.Count; colI++)
            {
                B.Add(new int[bRowCount]);
                for (int rowI = 0; rowI < bRowCount; rowI++)
                {
                    B[colI][rowI] = eM[colI][cRowCount + rowI];
                }
            }
            //phase 2
            for (int h = 0; h < bRowCount; h++)
            {
                List<int> posInds = new List<int>();
                List<int> negInds = new List<int>();
                for (int colI = 0; colI < B.Count; colI++)
                {
                    if (B[colI][h] > 0)
                    {
                        posInds.Add(colI);
                    }
                    else if (B[colI][h] < 0)
                    {
                        negInds.Add(colI);
                    }
                }
                if (negInds.Count > 0)
                {
                    if (posInds.Count > 0)
                    {
                        for (int j = 0; j < posInds.Count; j++)//for each pair
                        {
                            for (int k = 0; k < negInds.Count; k++)
                            {
                                //to match coefficients
                                int jAbs = Math.Abs(B[posInds[j]][h]);
                                int kAbs = Math.Abs(B[negInds[k]][h]);
                                int gcd = GCD(jAbs, kAbs);
                                int finalCoef = jAbs / gcd * kAbs;
                                int jMult = finalCoef / jAbs;
                                int kMult = finalCoef / kAbs;
                                if (posInds[j] * jMult == negInds[k] * kMult) kMult *= -1;
                                int[] newColumn = new int[bRowCount];
                                for (int rowI = 0; rowI < newColumn.Length; rowI++)
                                {
                                    newColumn[rowI] = B[posInds[j]][rowI] * jMult + B[negInds[k]][rowI] * kMult;
                                }
                                int columnGCD = Math.Abs(GCDArray(newColumn));
                                for (int i = 0; i < newColumn.Length; i++) newColumn[i] /= columnGCD;
                                B.Add(newColumn);
                            }
                        }
                        //support calculation
                        for (int chk = 0; chk < B.Count; chk++)
                        {
                            bool hasMinSupp = true;
                            for (int comp = 0; comp < B.Count; comp++)
                            {
                                bool compFine = false;
                                for (int rowI = 0; rowI < bRowCount; rowI++)
                                {
                                    if (B[chk][rowI] == 0 && B[comp][rowI] != 0)
                                    {
                                        compFine = true;
                                        break;
                                    }
                                }
                                if (!compFine)
                                {
                                    for (int rowI = 0; rowI < bRowCount; rowI++)
                                    {
                                        if (B[chk][rowI] != 0 && B[comp][rowI] == 0)
                                        {
                                            hasMinSupp = false;
                                            break;
                                        }
                                    }
                                    if (!hasMinSupp) break;
                                }
                            }
                            if (!hasMinSupp)
                            {
                                B.RemoveAt(chk);
                                chk--;
                            }
                        }
                    }
                    for (int negIndI = negInds.Count - 1; negIndI >= 0; negIndI--)
                    {
                        B.RemoveAt(negInds[negIndI]);
                    }
                }
            }

            //converting
            int[,] solutions = new int[B.Count, bRowCount];
            for (int rowI = 0; rowI < solutions.GetLength(0); rowI++)
            {
                for (int colI = 0; colI < solutions.GetLength(1); colI++)
                {
                    solutions[rowI, colI] = B[rowI][colI];
                }
            }
            return solutions;
        }


        private int[,] CheckInvariantsTSS(int[,] w)
        {
            int varCount = w.GetLength(1);
            int eqCount = w.GetLength(0);
            List<int[]> e = new List<int[]>();//canonical basis vectors
            
            for (int i = 0; i < varCount; i++)
            {
                e.Add(new int[varCount]);//new canonical basis vector
                e[i][i] = 1;//giving it value
            }
            for (int i = 0; i < eqCount; i++)
            {
                int[] L = new int[varCount];//current function (row of the matrix)
                for (int v = 0; v < varCount; v++)
                {
                    L[v] = w[i, v];//transfering first equation to L function
                }
                List<int> eResults = new List<int>();//results of L with e
                for (int j = 0; j < e.Count; j++)
                {
                    int sum = 0;
                    for (int v = 0; v < varCount; v++)
                    {
                        sum += L[v] * e[j][v];
                    }
                    eResults.Add(sum);
                }
                //splitting vectors into 3 sets
                List<int> Mpos = new List<int>();//these lists contain INDEXES from e
                List<int> Mneg = new List<int>();
                List<int> Mzer = new List<int>();
                for (int e_ = 0; e_ < e.Count; e_++)
                {
                    if (eResults[e_] > 0)
                    {
                        Mpos.Add(e_);
                    }
                    else if (eResults[e_] < 0)
                    {
                        Mneg.Add(e_);
                    }
                    else
                    {
                        Mzer.Add(e_);
                    }
                }//
                //building new M
                List<int[]> Mnew = new List<int[]>();//contains VECTORS
                for (int negI = 0; negI < Mneg.Count; negI++)//matrix of vectors
                {
                    for (int posI = 0; posI < Mpos.Count; posI++)
                    {
                        int[] y = new int[varCount];
                        for (int v = 0; v < varCount; v++)//big formula
                        {
                            y[v] = -eResults[Mneg[negI]] * e[Mpos[posI]][v] + eResults[Mpos[posI]] * e[Mneg[negI]][v];
                        }
                        Mnew.Add(y);
                    }
                }
                for (int zerI = 0; zerI < Mzer.Count; zerI++)//adding all zero vectors
                {
                    Mnew.Add(e[Mzer[zerI]]);
                }
                e = Mnew;//now solutions of first equation are input data for second
                         //removing redundant solutions
                         //finding t(the biggest coefficient)
                int t = 0;
                for (int i_ = 0; i_ < e.Count; i_++)
                {
                    int eqMax = e[i_].Max();
                    if (eqMax > t)
                    {
                        t = eqMax;
                    }
                }
                for (int i_ = 0; i_ < e.Count; i_++)//check every solution
                {
                    int[] tx = new int[varCount];//current solution multipled by t
                    for (int j = 0; j < varCount; j++)
                    {
                        tx[j] = e[i_][j] * t;
                    }
                    bool isRedundant = false;
                    for (int j = 0; j < e.Count; j++)//check with every other solution
                    {
                        if (j == i_) continue;
                        bool isBigger = true;
                        for (int k = 0; k < varCount; k++)
                        {
                            if (tx[k] < e[j][k])
                            {
                                isBigger = false;
                                break;
                            }
                        }
                        if (isBigger)
                        {
                            isRedundant = true;
                            break;
                        }
                    }
                    if (isRedundant)
                    {
                        e.RemoveAt(i_);
                        i_--;
                    }
                }
            }
            
            int[,] solutions = new int[e.Count, varCount];
            for (int i = 0; i < e.Count; i++)
            {
                for (int j = 0; j < varCount; j++)
                {
                    solutions[i, j] = e[i][j];
                }
            }
            return solutions;
        }

        private int[,] CheckInvariantsTSSOpt(int[,] w)
        {
            int varCount = w.GetLength(1);
            int eqCount = w.GetLength(0);
            List<int[]> e = new List<int[]>();//canonical basis vectors

            for (int i = 0; i < varCount; i++)
            {
                e.Add(new int[varCount]);//new canonical basis vector
                e[i][i] = 1;//giving it value
            }
            for (int i = 0; i < eqCount; i++)
            {
                int[] L = new int[varCount];//current function (row of the matrix)
                for (int v = 0; v < varCount; v++)
                {
                    L[v] = w[i, v];//transfering first equation to L function
                }
                List<int> eResults = new List<int>();//results of L with e
                for (int j = 0; j < e.Count; j++)
                {
                    int sum = 0;
                    for (int v = 0; v < varCount; v++)
                    {
                        sum += L[v] * e[j][v];
                    }
                    eResults.Add(sum);
                }
                //splitting vectors into 3 sets
                List<int> Mpos = new List<int>();//these lists contain INDEXES from e
                List<int> Mneg = new List<int>();
                List<int> Mzer = new List<int>();
                for (int e_ = 0; e_ < e.Count; e_++)
                {
                    if (eResults[e_] > 0)
                    {
                        Mpos.Add(e_);
                    }
                    else if (eResults[e_] < 0)
                    {
                        Mneg.Add(e_);
                    }
                    else
                    {
                        Mzer.Add(e_);
                    }
                }//
                //building new M
                List<int[]> Mnew = new List<int[]>();//contains VECTORS
                for (int negI = 0; negI < Mneg.Count; negI++)//matrix of vectors
                {
                    for (int posI = 0; posI < Mpos.Count; posI++)
                    {
                        int[] y = new int[varCount];
                        for (int v = 0; v < varCount; v++)//big formula
                        {
                            y[v] = -eResults[Mneg[negI]] * e[Mpos[posI]][v] + eResults[Mpos[posI]] * e[Mneg[negI]][v];
                        }
                        Mnew.Add(y);
                    }
                }
                for (int zerI = 0; zerI < Mzer.Count; zerI++)//adding all zero vectors
                {
                    Mnew.Add(e[Mzer[zerI]]);
                }
                e = Mnew;//now solutions of first equation are input data for second
                         //removing redundant solutions
                         //finding t(the biggest coefficient)
                int t = 0;
                for (int i_ = 0; i_ < e.Count; i_++)
                {
                    int eqMax = e[i_].Max();
                    if (eqMax > t)
                    {
                        t = eqMax;
                    }
                }
                for (int i_ = 0; i_ < e.Count; i_++)//check every solution
                {
                    int[] tx = new int[varCount];//current solution multipled by t
                    for (int j = 0; j < varCount; j++)
                    {
                        tx[j] = e[i_][j] * t;
                    }
                    bool isRedundant = false;
                    for (int j = 0; j < e.Count; j++)//check with every other solution
                    {
                        if (j == i_) continue;
                        bool isBigger = true;
                        for (int k = 0; k < varCount; k++)
                        {
                            if (tx[k] < e[j][k])
                            {
                                isBigger = false;
                                break;
                            }
                        }
                        if (isBigger)
                        {
                            isRedundant = true;
                            break;
                        }
                    }
                    if (isRedundant)
                    {
                        e.RemoveAt(i_);
                        i_--;
                    }
                }
            }
            int[,] solutions = new int[e.Count, varCount];
            for (int i = 0; i < e.Count; i++)
            {
                for (int j = 0; j < varCount; j++)
                {
                    solutions[i, j] = e[i][j];
                }
            }
            return solutions;
        }

        private int[,] CheckInvariantsTSSMod(int[,] w)
        {
            int varCount = w.GetLength(1);
            int eqCount = w.GetLength(0);
            List<int[]> e = new List<int[]>();//canonical basis vectors

            for (int i = 0; i < varCount; i++)
            {
                e.Add(new int[varCount]);//new canonical basis vector
                e[i][i] = 1;//giving it value
            }
            for (int i = 0; i < eqCount; i++)
            {
                int[] L = new int[varCount];//current function (row of the matrix)
                for (int v = 0; v < varCount; v++)
                {
                    L[v] = w[i, v];//transfering first equation to L function
                }
                List<int> eResults = new List<int>();//results of L with e
                for (int j = 0; j < e.Count; j++)
                {
                    int sum = 0;
                    for (int v = 0; v < varCount; v++)
                    {
                        sum += L[v] * e[j][v];
                    }
                    eResults.Add(sum);
                }
                //splitting vectors into 3 sets
                List<int> Mpos = new List<int>();//these lists contain INDEXES from e
                List<int> Mneg = new List<int>();
                List<int> Mzer = new List<int>();
                for (int e_ = 0; e_ < e.Count; e_++)
                {
                    if (eResults[e_] > 0)
                    {
                        Mpos.Add(e_);
                    }
                    else if (eResults[e_] < 0)
                    {
                        Mneg.Add(e_);
                    }
                    else
                    {
                        Mzer.Add(e_);
                    }
                }//
                //building new M
                List<int[]> Mnew = new List<int[]>();//contains VECTORS
                for (int negI = 0; negI < Mneg.Count; negI++)//matrix of vectors
                {
                    for (int posI = 0; posI < Mpos.Count; posI++)
                    {
                        int[] y = new int[varCount];
                        for (int v = 0; v < varCount; v++)//big formula
                        {
                            y[v] = -eResults[Mneg[negI]] * e[Mpos[posI]][v] + eResults[Mpos[posI]] * e[Mneg[negI]][v];
                        }
                        Mnew.Add(y);
                    }
                }
                for (int zerI = 0; zerI < Mzer.Count; zerI++)//adding all zero vectors
                {
                    Mnew.Add(e[Mzer[zerI]]);
                }
                e = Mnew;//now solutions of first equation are input data for second
            }

            int[,] solutions = new int[e.Count, varCount];
            for (int i = 0; i < e.Count; i++)
            {
                for (int j = 0; j < varCount; j++)
                {
                    solutions[i, j] = e[i][j];
                }
            }
            return solutions;
        }

        private int GCD(int a, int b)
        {
            if (a == 0)
                return b;

            return GCD(b % a, a);
        }

        private int GCDArray(int[] numbers)
        {
            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(result, numbers[i]);
                if (result == 1) // Якщо знайдено НСД = 1, можна припинити пошук, оскільки інший спільний дільник неможливий.
                    return 1;
            }
            return result;
        }
    }
}