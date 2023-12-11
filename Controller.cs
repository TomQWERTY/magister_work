using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

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
                view.ModifyTokens(i, newVal);
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

        public void Analyze()
        {
            AnalysisType at = view.GetAnalysisMethod();
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
                bool tInv = true;
                int[,] tInvs;
                if (at == AnalysisType.TSS)
                {
                    tInvs = CheckInvariantsTSS(w, ref tInv);
                }
                else if (at == AnalysisType.AlOpt)
                {
                    tInvs = CheckInvariantsAlOpt(w, ref tInv);
                }
                else
                {
                    tInvs = CheckInvariants(w, ref tInv);
                }
                w = new int[model.matrixW[0].Count, model.matrixW.Count];
                for (int i = 0; i < w.GetLength(0); i++)
                {
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        w[i, j] = model.matrixW[j][i];
                    }
                }
                bool pInv = true;
                int[,] pInvs = new int[1,1];
                if (at == AnalysisType.TSS)
                {
                    long sum = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        DateTime dt1 = DateTime.Now;
                        pInvs = CheckInvariantsTSS(w, ref pInv);
                        sum += (DateTime.Now - dt1).Milliseconds;
                    }
                    MessageBox.Show((sum * 1.0 / 1000) + "");
                }
                else if (at == AnalysisType.AlOpt)
                {
                    /*long sum = 0;
                    for (int i = 0; i < 1000; i++)
                    {
                        DateTime dt1 = DateTime.Now;*/
                        pInvs = CheckInvariantsAlOpt(w, ref pInv);
                       /* sum += (DateTime.Now - dt1).Ticks;
                    }
                    MessageBox.Show((sum * 1.0 / 1000) + "");*/
                }
                else
                {
                    pInvs = CheckInvariants(w, ref pInv);
                }
                bool[] infoForView = new bool[6];
                if (tInv)
                {
                    infoForView[0] = true;
                    infoForView[1] = true;
                    infoForView[2] = true;
                }
                if (pInv)
                {
                    infoForView[3] = true;
                    infoForView[4] = true;
                }
                w = new int[model.matrixW[0].Count, model.matrixW.Count];
                for (int i = 0; i < w.GetLength(0); i++)
                {
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        w[i, j] = model.matrixW[j][i];
                    }
                }
                int rank = FindRank(w);
                if (rank >= Math.Min(model.matrixW.Count, model.matrixW[0].Count))
                {
                    infoForView[5] = true;
                }
                view.ShowResults(infoForView, rank, tInvs, pInvs);
            }
        }

        private int[,] CheckInvariantsAlOpt(int[,] c, ref bool ok)
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
            if (solutions.Length > 0)//copide from TSS
            {
                //reduce coefficients
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
                        solutions[v, p] /= gcdArr;
                    }
                }
                //check dynamic properties
                bool[] covered = new bool[solutions.GetLength(1)];
                for (int i = 0; i < solutions.GetLength(1); i++)
                {
                    for (int j = 0; j < solutions.GetLength(0); j++)
                    {
                        if (solutions[j, i] != 0)
                        {
                            covered[i] = true;
                            break;
                        }
                    }
                    if (!covered[i])
                    {
                        ok = false;
                        break;
                    }
                }
            }
            else
            {
                ok = false;
                solutions = new int[1, solutions.GetLength(1)];
                for (int i = 0; i < solutions.GetLength(1); i++)
                {
                    solutions[0, i] = 0;
                }
            }
            return solutions;
        }


        private int[,] CheckInvariantsTSS(int[,] w, ref bool ok)
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
            //removing redundant solutions
            //finding t(the biggest coefficient)
            int t = 0;
            for (int i = 0; i < e.Count; i++)
            {
                int eqMax = e[i].Max();
                if (eqMax > t)
                {
                    t = eqMax;
                }
            }
            for (int i = 0; i < e.Count; i++)//check every solution
            {
                int[] tx = new int[varCount];//current solution multipled by t
                for (int j = 0; j < varCount; j++)
                {
                    tx[j] = e[i][j] * t;
                }
                bool isRedundant = false;
                for (int j = 0; j < e.Count; j++)//check with every other solution
                {
                    if (j == i) continue;
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
                    e.RemoveAt(i);
                    i--;
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
            if (solutions.Length > 0)
            {
                //reduce coefficients
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
                        solutions[v, p] /= gcdArr;
                    }
                }
                //check dynamic properties
                bool[] covered = new bool[varCount];
                for (int i = 0; i < varCount; i++)
                {
                    for (int j = 0; j < solutions.GetLength(0); j++)
                    {
                        if (solutions[j, i] != 0)
                        {
                            covered[i] = true;
                            break;
                        }
                    }
                    if (!covered[i])
                    {
                        ok = false;
                        break;
                    }
                }
            }
            else
            {
                ok = false;
                solutions = new int[1, varCount];
                for (int i = 0; i <  solutions.GetLength(1); i++)
                {
                    solutions[0, i] = 0;
                }
            }
            return solutions;
        }

        private int[,] CheckInvariants(int[,] w, ref bool ok)
        {
            //avoiding losing variables
            bool allGood = true;
            do
            {
                allGood = true;
                for (int j = 0; j < w.GetLength(1); j++)
                {
                    int sum = 0;
                    int nonZero = 0;
                    for (int i = 0; i < w.GetLength(0); i++)
                    {
                        sum += w[i, j];
                        if (w[i, j] != 0) nonZero = i;
                    }
                    if (sum == 0)
                    {
                        //allGood = false;
                        for (int k = 0; k < w.GetLength(1); k++)
                        {
                            //w[nonZero, k] *= 2;
                        }
                        break;
                    }
                }
            }
            while (!allGood);

            int[] equation = new int[w.GetLength(1)];
            int varCount = w.GetLength(1);
            int equationCount = w.GetLength(0);
            int[,] parametredRes = new int[equation.Length, 1 + (varCount - equationCount > 1 ? varCount - equationCount : 1)];
            //bool square = w.GetLength(0) == w.GetLength(1);

            if (true)
            {
                int[,,] reps = new int[equationCount, varCount, Math.Max(1, varCount - equationCount)];
                for (int i = 0; i < varCount - 1 && i < equationCount; i++)
                {
                    reps[i, i, 0] = w[i, i];
                    //every other variable
                    for (int k = i + 1; k < varCount; k++)
                    {
                        reps[i, k, 0] = -w[i, k];
                    }
                    //every other equation
                    for (int j = i + 1; j < equationCount; j++)
                    {
                        //every other variable
                        for (int k = i + 1; k < varCount; k++)
                        {
                            w[j, k] *= reps[i, i, 0];
                            w[j, k] += reps[i, k, 0] * w[j, i];
                        }
                        w[j, i] = 0;
                    }
                }
                if (varCount - equationCount > 1)//using Diophantine equation algorithm
                {
                    int[] equation2 = new int[varCount - equationCount + 1];
                    for (int i = 0; i < equation2.Length; i++)
                    {
                        //equation2[i] = reps[reps.GetLength(0) - 1, i + (reps.GetLength(1) - equation2.Length), 0];
                        equation2[i] = w[reps.GetLength(0) - 1, i + (reps.GetLength(1) - equation2.Length)];
                    }
                    //equation2[0] *= -1;

                    int[,] parametredRes2 = new int[equation2.Length, equation2.Length];
                    int[] initialRes1 = new int[equation2.Length];
                    int[] initialRes2 = new int[equation2.Length];
                    int[] freeEl = new int[equation2.Length];
                    for (int i = 0; i < equation2.Length - 1; i++)
                    {
                        int first = 1;
                        if (i == equation2.Length - 2)
                        {
                            first = equation2[equation2.Length - 2 - i];
                        }
                        for (int j = 0; j <= i; j++)
                        {
                            if (j == 0)
                            {
                                int sol1 = 1, sol2 = 1;
                                int gcd1 = GCDExtended(first, equation2[equation2.Length - 1 - i],
                                    ref sol1, ref sol2);
                                if (gcd1 < 0) gcd1 = -gcd1;
                                else if (gcd1 == 0) gcd1 = 1;
                                initialRes1[j] = sol1 * (freeEl[j] / gcd1);
                                initialRes2[j] = sol2 * (freeEl[j] / gcd1);
                            }
                            else
                            {
                                initialRes2[j] = 1;
                                int coeficient1 = equation2[equation2.Length - 2 - i];
                                initialRes1[j] = coeficient1 != 0 ?
                                    (freeEl[j] - initialRes2[j] * equation2[equation2.Length - 1 - i]) / coeficient1 : 0;
                            }
                            /*if (j == 0 && equation[equation.Length - 2 - i] > 1)
                            {
                                initialRes1[j] = 1;
                                initialRes2[j] = freeEl[j] - initialRes1[j] * equation[equation.Length - 2 - i];
                            }*/
                            parametredRes2[equation2.Length - 1 - i, j] = initialRes2[j];
                            freeEl[j] = initialRes1[j];
                        }
                        int gcd = GCD(first, equation2[equation2.Length - 1 - i]);
                        if (gcd < 0) gcd = -gcd;
                        else if (gcd == 0) gcd = 1;
                        parametredRes2[equation2.Length - 1 - i, i + 1] = -first / gcd;//new parameter
                        gcd = GCD(first, equation2[equation2.Length - 1 - i]);
                        if (gcd < 0) gcd = -gcd;
                        else if (gcd == 0) gcd = 1;
                        freeEl[i + 1] = equation2[equation2.Length - 1 - i] / gcd;
                    }
                    for (int i = 0; i < freeEl.Length; i++)
                    {
                        parametredRes2[0, i] = freeEl[i];
                    }
                    int a = 0;
                    for (int e = 0; e < equationCount - 1; e++)//write values of found variables
                    {
                        for (int p = 1; p < parametredRes2.GetLength(1); p++)//all parameters of last variable
                        {
                            reps[e, varCount - 1, p - 1] *= parametredRes2[parametredRes2.GetLength(0) - 1, p];
                        }
                        for (int v = 0; v < parametredRes2.GetLength(0) - 1; v++)//all variables found except last one
                        {
                            int vInReps = v + (varCount - equation2.Length);
                            for (int p = 1; p < parametredRes2.GetLength(1); p++)//all parameters
                            {
                                reps[e, varCount - 1, p - 1] += parametredRes2[v, p] * reps[e, vInReps, 0];
                            }
                            reps[e, vInReps, 0] = 0;
                            //reps[e, varCount - 1, 0] *= parametredRes2[v, 0 + 1];
                        }
                    }
                    //write to final result
                    for (int v = 0; v < parametredRes2.GetLength(0); v++)//all variables found
                    {
                        int vInReps = v + (varCount - equation2.Length);
                        for (int p = 1; p < parametredRes2.GetLength(1); p++)//all parameters
                        {
                            parametredRes[vInReps, p] = parametredRes2[v, p];
                        }
                    }
                    a = 0;
                }
                //backwards
                if (varCount - equationCount > 1)
                {
                    for (int i = varCount - (varCount - equationCount + 2); i > 0; i--)
                    {
                        //every other replace equation
                        for (int j = i - 1; j >= 0; j--)
                        {
                            //every other variable
                            for (int k = varCount - 1; k >= 0; k--)
                            {
                                for (int p = 0; p < reps.GetLength(2); p++)
                                {
                                    if (k != i) reps[j, k, p] *= reps[i, i, 0];
                                }
                            }
                            for (int p = 0; p < reps.GetLength(2); p++)
                            {
                                reps[j, varCount - 1, p] += reps[j, i, 0] * reps[i, varCount - 1, p];
                            }
                            reps[j, i, 0] = 0;
                        }
                    }
                }
                else
                {
                    for (int i = varCount - 2; i > 0; i--)
                    {
                        //every other replace equation
                        for (int j = i - 1; j >= 0; j--)
                        {
                            //every other variable
                            for (int k = varCount - 1; k >= 0; k--)
                            {
                                if (k != i) reps[j, k, 0] *= reps[i, i, 0];
                            }
                            reps[j, varCount - 1, 0] += reps[j, i, 0] * reps[i, varCount - 1, 0];
                            reps[j, i, 0] = 0;
                        }
                    }
                }
                //reduce reps
                List<int> repsL = new List<int>();
                foreach (int el in reps) repsL.Add(el);
                int gcdR = GCDArray(repsL.ToArray());
                for (int i = 0; i < reps.GetLength(0); i++)
                {
                    for (int j = 0; j < reps.GetLength(1); j++)
                    {
                        for (int k = 0; k < reps.GetLength(2); k++)
                        {
                            if (gcdR != 0) reps[i, j, k] /= gcdR;
                        }
                    }
                }



                if (varCount - equationCount > 1)
                {
                    int multiplier = 1;
                    for (int i = 0; i < varCount - (varCount - equationCount + 1); i++)
                    {
                        multiplier *= reps[i, i, 0];
                    }
                    for (int i = 0; i < equationCount; i++)//all
                    {
                        for (int p = 0; p < reps.GetLength(2); p++)
                        {
                            reps[i, varCount - 1, p] *= multiplier;
                        }
                    }
                    for (int v = varCount - (varCount - equationCount + 1); v < parametredRes.GetLength(0); v++)
                    {
                        for (int p = 0; p < reps.GetLength(2); p++)
                        {
                            parametredRes[v, p + 1] *= multiplier;
                        }
                    }
                    for (int i = 0; i < varCount - (varCount - equationCount + 1); i++)
                    {
                        if (reps[i, i, 0] != 0)
                        {
                            for (int p = 1; p < parametredRes.GetLength(1); p++)
                            {
                                parametredRes[i, p] = reps[i, varCount - 1, p - 1] / reps[i, i, 0];
                            } 
                        }
                    }
                }
                else
                {
                    int multiplier = 1;
                    for (int i = 0; i < varCount - 1; i++)
                    {
                        multiplier *= reps[i, i, 0];
                    }
                    for (int i = 0; i < varCount - 1; i++)
                    {
                        reps[i, varCount - 1, 0] *= multiplier;
                    }
                    for (int i = 0; i < varCount - 1; i++)
                    {
                        if (reps[i, i, 0] != 0) parametredRes[i, 1] = reps[i, varCount - 1, 0] / reps[i, i, 0];
                    }
                    parametredRes[varCount - 1, 1] = multiplier;
                }
            }
            else
            {
                //int[] equation = new int[w.GetLength(1)];
                for (int i = 0; i < equation.Length; i++)
                {
                    equation[i] = 0;
                    for (int j = 0; j < w.GetLength(0); j++)
                    {
                        equation[i] += w[j, i];
                    }
                }

                //int[,] parametredRes = new int[equation.Length, equation.Length];
                int[] initialRes1 = new int[equation.Length];
                int[] initialRes2 = new int[equation.Length];
                int[] freeEl = new int[equation.Length];
                for (int i = 0; i < equation.Length - 1; i++)
                {
                    int first = 1;
                    if (i == equation.Length - 2)
                    {
                        first = equation[equation.Length - 2 - i];
                    }
                    for (int j = 0; j <= i; j++)
                    {
                        if (j == 0)
                        {
                            int sol1 = 1, sol2 = 1;
                            int gcd = GCDExtended(first, equation[equation.Length - 1 - i],
                                ref sol1, ref sol2);
                            initialRes1[j] = sol1 * (freeEl[j] / gcd);
                            initialRes2[j] = sol2 * (freeEl[j] / gcd);
                        }
                        else
                        {
                            initialRes2[j] = 1;
                            initialRes1[j] = freeEl[j] - initialRes2[j] * equation[equation.Length - 1 - i];
                        }
                        /*if (j == 0 && equation[equation.Length - 2 - i] > 1)
                        {
                            initialRes1[j] = 1;
                            initialRes2[j] = freeEl[j] - initialRes1[j] * equation[equation.Length - 2 - i];
                        }*/
                        parametredRes[equation.Length - 1 - i, j] = initialRes2[j];
                        freeEl[j] = initialRes1[j];
                    }
                    parametredRes[equation.Length - 1 - i, i + 1] = -first /
                        GCD(first, equation[equation.Length - 1 - i]);//new parameter
                    freeEl[i + 1] = equation[equation.Length - 1 - i] /
                        GCD(first, equation[equation.Length - 1 - i]);
                }
                for (int i = 0; i < freeEl.Length; i++)
                {
                    parametredRes[0, i] = freeEl[i];
                }
            }

            //reduce parameters values
            int[] allParVals = new int[parametredRes.GetLength(0) * (parametredRes.GetLength(1) - 1)];
            for (int v = 0, i = 0; v < parametredRes.GetLength(0); v++)
            {
                for (int p = 1; p <  parametredRes.GetLength(1); p++, i++)
                {
                    allParVals[i] = parametredRes[v, p];
                }
            }
            int gcdArr = GCDArray(allParVals);
            for (int v = 0; v < parametredRes.GetLength(0); v++)
            {
                for (int p = 1; p < parametredRes.GetLength(1); p++)
                {
                    if (gcdArr != 0) parametredRes[v, p] /= gcdArr;
                }
            }

            Random rnd = new Random();
            bool[] covered = new bool[equation.Length];
            int tries = 10000;
            int negs = 0;
            List<int[]> remSolutions = new List<int[]>();
            for (int i = 0; i < tries; i++)
            {
                int[] parameters = new int[equation.Length - 1];
                for (int j = 0; j < parameters.Length; j++)
                {
                    if (i > tries / 2)
                    {
                        parameters[j] = i + 1;
                    }
                    else
                    {
                        parameters[j] = rnd.Next(-10, 11);
                    }
                }
                //if (parameters.Length > 1 && (parameters[0] < 0 || parameters[1] < 0)) MessageBox.Show("found");
                int[] sol = new int[parametredRes.GetLength(0)];
                bool negPresent = false;
                for (int j = 0; j < parametredRes.GetLength(0); j++)//forming new solution variant
                {
                    int sum = parametredRes[j, 0];
                    for (int k = 1; k < parametredRes.GetLength(1); k++)
                    {
                        sum += parametredRes[j, k] * parameters[k - 1];
                    }
                    sol[j] = sum;
                    if (sum < 0)
                    {
                        negPresent = true;
                        break;
                    }
                }
                if (negPresent)
                {
                    negs++;
                    i--;
                    if (negs > 20000) break;
                    continue;
                }
                bool solves = true;
                for (int j = 0; j < w.GetLength(0); j++)//checking if it actually solves
                {
                    int sum = 0;
                    for (int k = 0; k < w.GetLength(1); k++) sum += sol[k] * w[j, k];
                    if (sum != 0)
                    {
                        solves = false;
                        break;
                    }
                }
                if (solves)
                {
                    bool allZeros = true;
                    for (int j = 0; j < sol.Length; j++)//checking if covers
                    {
                        if (sol[j] != 0)
                        {
                            covered[j] = true;
                            allZeros = false;
                        }
                    }
                    if (!allZeros) remSolutions.Add(sol);
                }
            }
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i])
                {
                    ok = false;
                    break;
                }
            }
            int[,] results = new int[remSolutions.Count, parametredRes.GetLength(0)];
            for (int i = 0; i < remSolutions.Count; i++)
            {
                for (int j = 0; j < remSolutions[i].Length; j++) results[i, j] = remSolutions[i][j];
            }
            return results;
        }

        private int FindRankOld(int[,] mat)
        {
            int rank = mat.GetLength(1);
            for (int row = 0; row < Math.Min(rank, mat.GetLength(0)); row++)
            {
                if (mat[row, row] != 0)
                {
                    for (int col = 0; col < Math.Min(mat.GetLength(1), mat.GetLength(0)); col++)
                    {
                        if (col != row)
                        {
                            double mult =
                            (double)mat[col, row] /
                                        mat[row, row];

                            for (int i = 0; i < rank; i++)

                                mat[col, i] -= (int)mult
                                        * mat[row, i];
                        }
                    }
                }
                else
                {
                    bool reduce = true;
                    for (int i = row + 1; i < mat.GetLength(0); i++)
                    {
                        if (mat[i, row] != 0)
                        {
                            SwapRows(mat, row, i, rank);
                            reduce = false;
                            break;
                        }
                    }
                    if (reduce)
                    {
                        rank--;
                        for (int i = 0; i < mat.GetLength(0); i++)
                            mat[i, row] = mat[i, rank];
                    }
                    row--;
                }
            }
            return rank;
        }

        private int FindRank(int[,] mat)
        {
            double[,] matrix = new double[mat.GetLength(0), mat.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++) matrix[i, j] = mat[i, j];
            }

            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            int rank = 0;

            // Apply Gaussian elimination
            for (int i = 0; i < Math.Min(rowCount, colCount); i++)
            {
                // Find the pivot element
                int pivotRow = -1;
                if (matrix[i, i] != 0)
                {
                    pivotRow = i;
                }
                /*for (int j = 0; j < colCount; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        pivotRow = i;
                        break;
                    }
                }*/

                // If a pivot element is found
                if (pivotRow != -1)
                {
                    // Increment the rank
                    rank++;

                    // Normalize the pivot row if the pivot value is not zero
                    double pivotValue = matrix[pivotRow, pivotRow];
                    for (int j = 0; j < colCount; j++)
                    {
                        matrix[pivotRow, j] /= pivotValue;
                    }

                    // Eliminate non-zero elements in the current column
                    for (int k = 0; k < rowCount; k++)
                    {
                        if (k != pivotRow)
                        {
                            double factor = matrix[k, i];
                            for (int j = 0; j < colCount; j++)
                            {
                                matrix[k, j] -= factor * matrix[pivotRow, j];
                            }
                        }
                    }
                }
            }

            return rank;
        }

        private void SwapRows(int[,] mat, int row1, int row2, int col)
        {
            for (int i = 0; i < col; i++)
            {
                int temp = mat[row1, i];
                mat[row1, i] = mat[row2, i];
                mat[row2, i] = temp;
            }
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

        private int GCDExtended(int a, int b, ref int x, ref int y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            int x1 = 1, y1 = 1;
            int gcd = GCDExtended(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }
    }
}
