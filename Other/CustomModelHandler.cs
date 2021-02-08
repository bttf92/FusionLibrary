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

        public Model Request() => Utils.LoadAndRequestModel(Model);

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
            var fields = type.GetFields();
            var models = new List<CustomModel>();

            foreach (var field in fields)
            {
                var obj = field.GetValue(null);
                if (obj.GetType() == typeof(CustomModel))
                {
                    var modelObj = (CustomModel)obj;
                    models.Add(modelObj);
                }
                else if (obj.GetType() == typeof(Dictionary<int, CustomModel>))
                {
                    var dict = (Dictionary<int, CustomModel>)obj;
                    models.AddRange(dict.Values);
                }
            }

            return models;
        }
    }
}
