using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            view.ModifyTokens(ind, num, true);
        }

        public void Save(string fileName, ViewSaveData viewSaveData)
        {
            SaveData save = new SaveData();
            save.marking = marking;
            save.matrix = matrixW;
            save.viewSaveData = viewSaveData;
            File.WriteAllText(fileName, JsonConvert.SerializeObject(save, Formatting.Indented));
        }

        public void Load(string fileName, out ViewSaveData viewSaveData)
        {
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(fileName));
            marking = saveData.marking;
            matrixW = saveData.matrix;
            viewSaveData = saveData.viewSaveData;
        }

        public bool TryDeleteLastPlace()
        {
            if (matrixW.Count == 1) return false;
            bool allZeros = true;
            for (int i = 0; i < matrixW.Last().Count; i++)
            {
                if (matrixW.Last()[i] != 0)
                {
                    allZeros = false;
                    break;
                }
            }
            if (allZeros)
            {
                matrixW.RemoveAt(matrixW.Count - 1);
                marking.RemoveAt(marking.Count - 1);
            }
            return allZeros;
        }

        public bool TryDeleteLastTransition()
        {
            bool allZeros = true;
            for (int i = 0; i < matrixW.Count; i++)
            {
                if (matrixW[i][matrixW[0].Count - 1] != 0)
                {
                    allZeros = false;
                    break;
                }
            }
            if (allZeros)
            {
                for (int i = 0; i < matrixW.Count; i++)
                {
                    matrixW[i].RemoveAt(matrixW[0].Count - 1);
                }
            }
            return allZeros;
        }

        public void UpdateTokens()
        {
            for (int i = 0; i < marking.Count; i++)
            {
                view.ModifyTokens(i, marking[i], false);
            }
        }
        
        public void UpdateView()
        {
            int initialCount = marking.Count / 2;
            for (int i = marking.Count - 1; i >= initialCount; i--)
            {
                marking.RemoveAt(i);
            }
            UpdateTokens();
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
