using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace PawnShop.Oracle.Extensions
{
    public static class ModelConverting
    {
        public static TModel ConvertToObject<TModel>(DbDataReader dataReader) where TModel : class, new() 
        {
            TModel model = new TModel();

            foreach (var item in model.GetType().GetProperties())
            {
                model.GetType().GetProperty(item.Name).SetValue(model, dataReader[(typeof(TModel).Name + "_" + item.Name).ToLower()], null);
            }

            return model;
        }
    }
}
