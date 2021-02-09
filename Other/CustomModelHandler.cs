using GTA;
using GTA.UI;
using System;
using System.Collections.Generic;

namespace FusionLibrary
{
    public class CustomModel
    {
        public string Name { get; }
        public Model Model { get; }

        public CustomModel(string name)
        {
            Name = name;
            Model = new Model(name);
        }

        public CustomModel(Model model)
        {
            Name = model.Hash.ToString();
            Model = model;
        }

        public Model Request()
        {
            return Utils.LoadAndRequestModel(Model);
        }

        public static implicit operator Model(CustomModel customModel)
        {
            return customModel.Request();
        }

        public static implicit operator string(CustomModel customModel)
        {
            return customModel.Name;
        }

        public static implicit operator CustomModel(Model model)
        {
            return new CustomModel(model);
        }
    }

    public class CustomModelHandler
    {
        protected static CustomModel PreloadModel(CustomModel customModel)
        {
            LoadingPrompt.Show("Loading: " + customModel);

            if (!customModel.Model.IsLoaded)
            {
                if (!customModel.Model.IsInCdImage || !customModel.Model.IsValid)
                    throw new Exception(customModel.Model + " not present!");

                Utils.LoadAndRequestModel(customModel);
            }

            LoadingPrompt.Hide();

            return customModel;
        }

        protected static List<CustomModel> GetAllModels(Type type)
        {
            System.Reflection.FieldInfo[] fields = type.GetFields();
            List<CustomModel> models = new List<CustomModel>();

            foreach (System.Reflection.FieldInfo field in fields)
            {
                object obj = field.GetValue(null);
                if (obj.GetType() == typeof(CustomModel))
                {
                    CustomModel modelObj = (CustomModel)obj;
                    models.Add(modelObj);
                }
                else if (obj.GetType() == typeof(Dictionary<int, CustomModel>))
                {
                    Dictionary<int, CustomModel> dict = (Dictionary<int, CustomModel>)obj;
                    models.AddRange(dict.Values);
                }
            }

            return models;
        }
    }
}
