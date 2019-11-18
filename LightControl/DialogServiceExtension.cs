using LightControl.Models;
using LightControl.ViewModels;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightControl
{
    public static class DialogServiceExtension
    {
        public const string EditLampDialog = nameof(EditLampDialog);
        public static void OpenLampEdit(this IDialogService dialogService, Lamp lamp)
        {
            if (dialogService is null)
                throw new ArgumentNullException(nameof(dialogService));

            var parameters = new DialogParameters();
            parameters.Add(nameof(LampEditDialogViewModel.Lamp), lamp);
            dialogService.Show(EditLampDialog, parameters, i => { });
        }
    }
}
