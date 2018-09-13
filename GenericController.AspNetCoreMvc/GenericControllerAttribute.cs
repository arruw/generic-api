using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenericController.AspNetMvc
{
    public class GenericControllerAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var entityType = controller.ControllerType.GetGenericArguments()[0];

            controller.ControllerName = entityType.Name;
        }
    }
}
