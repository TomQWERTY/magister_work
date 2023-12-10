using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CourseWork
{
    internal class Model
    {
        public List<List<int>> matrixW;
        public List<int> marking;
        ViewForm view;
        int transitionElementsToAdd;

        public Model(ViewForm vf)
        {
            matrixW = new List<List<int>>();
            marking = new List<int>();
            view = vf;
            transitionElementsToAdd = 0;
        }

        public void AddPlaceElement()
        {
            matrixW.Add(new List<int>());
            marking.Add(0);
            if (matrixW.Count > 1)
            {
                for (int i = 0; i < matrixW[0].Count; i++)
                {
                    matrixW[matrixW.Count - 1].Add(0);
                }
            }
            else
            {
                for (int i = 0; i < transitionElementsToAdd; i++)
                {
                    matrixW[matrixW.Count - 1].Add(0);
                }
                transitionElementsToAdd = 0;
            }
        }

        public void AddTransitionElement()
        {
            if (matrixW.Count > 0)
            {
                for (int i = 0; i < matrixW.Count; i++)
                {
                    matrixW[i].Add(0);
                }
            }
            else
            {
                transitionElementsToAdd++;
            }
        }

        public bool TransitionsPresent
        {
            get
            {
                return matrixW.Count > 0 && matrixW[0].Count > 0 || transitionElementsToAdd > 0;
            }
        }

        public void AddConection(bool placeToTrans, int ind1, int ind2, int val)
        {
            if (placeToTrans)
            {
                matrixW[ind1][ind2] = -val;
                view.ChangeMatrixValue(ind1, ind2, -val);
            }
            else
            {
                matrixW[ind2][ind1] = val;
                view.ChangeMatrixValue(ind2, ind1, val);
            }
        }

        public void AddTokens(int ind, int num)
        {
            marking[ind] = num;
            view.ModifyTokens(ind, num);
        }

        public void Save(string fileName, string[] viewOutput)
        {
            string[] output = new string[viewOutput.Length + matrixW.Count + 1];
            for (int i = 0; i <  viewOutput.Length; i++)
            {
                output[i] = viewOutput[i];
            }
            output[viewOutput.Length] = "";
            for (int i = 0; i < marking.Count;i++)
            {
                output[viewOutput.Length] += marking[i] + " ";
            }
            for (int i = 0; i < marking.Count; i++)
            {
                for (int j = 0; j < matrixW[i].Count; j++)
                {
                    output[viewOutput.Length + 1 + i] += matrixW[i][j] + " ";
                }
            }
            File.WriteAllLines(fileName, output);
        }

        public void Load(string fileName, out string[] viewOut)
        {
            string[] output = File.ReadAllLines(fileName);
            int connCount = Convert.ToInt32(output[3]);
            viewOut = new string[4 + connCount];
            for (int i = 0; i < viewOut.Length; i++)
            {
                viewOut[i] = output[i];
            }
            marking = new List<int>(Array.ConvertAll(output[viewOut.Length].Trim().Split(' '), Convert.ToInt32));
            matrixW = new List<List<int>>();
            for (int i = viewOut.Length + 1; i < output.Length; i++)
            {
                matrixW.Add(new List<int>(Array.ConvertAll(output[i].Trim().Split(' '), Convert.ToInt32)));
            }
        }

        public void UpdateView()
        {
            int initialCount = marking.Count / 2;
            for (int i = marking.Count - 1; i >= initialCount; i--)
            {
                marking.RemoveAt(i);
            }
            for (int i = 0; i < marking.Count; i++)
            {
                view.ModifyTokens(i, marking[i]);
            }
            initialCount = matrixW.Count / 2;
            for (int i = matrixW.Count - 1; i >= initialCount; i--)
            {
                matrixW.RemoveAt(i);
            }
            for (int i = 0; i < matrixW.Count; i++)
            {
                initialCount = matrixW[i].Count / 2;
                for (int j = matrixW[i].Count - 1; j >= initialCount; j--)
                {
                    matrixW[i].RemoveAt(j);
                }
                for (int j = 0; j < matrixW[i].Count; j++)
                {
                    view.ChangeMatrixValue(i, j, matrixW[i][j]);
                }
            }
        }
    }
}
