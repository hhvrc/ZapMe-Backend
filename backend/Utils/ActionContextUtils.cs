using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZapMe.Helpers;

namespace ZapMe.Utils;

public static class ActionContextUtils
{
    private static Dictionary<string, string[]> GetErrors(ModelStateDictionary modelState)
    {
        Dictionary<string, string[]> errors = new();

        foreach ((string key, ModelStateEntry value) in modelState)
        {
            ModelErrorCollection errorCollection = value.Errors;

            string[] entryErrors = new string[errorCollection.Count];

            for (int i = 0; i < errorCollection.Count; i++)
            {
                entryErrors[i] = errorCollection[i].ErrorMessage;
            }

            errors[key] = entryErrors;
        }

        return errors;
    }

    public static IActionResult CreateErrorResult(ActionContext actionContext)
    {
        ModelStateDictionary modelState = actionContext.ModelState;

        if (!modelState.IsValid)
        {
            return CreateHttpError.InvalidModelState(GetErrors(modelState)).ToActionResult();
        }

        return CreateHttpError.InternalServerError().ToActionResult();
    }
}
