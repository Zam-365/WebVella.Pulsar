﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using WebVella.Pulsar.Models;
using WebVella.Pulsar.Utils;
using System;
using WebVella.Pulsar.Services;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;

namespace WebVella.Pulsar.Components
{
	public partial class WvpInputHtml : WvpInputBase, IDisposable
	{

		#region << Parameters >>

		[Parameter] public string Value { get; set; } = "";

		#endregion

		#region << Callbacks >>
		[Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; } //Fires on each user input
		#endregion

		#region << Private properties >>

		private List<string> _cssList = new List<string>();

		private DotNetObjectReference<WvpInputHtml> _objectReference;

		private string _originalValue = "";

		private string _value = "";

		#endregion

		#region << Lifecycle methods >>

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				_objectReference = DotNetObjectReference.Create(this);
				await JsService.AddCKEditor(Id,_objectReference, "en");
			}
			await base.OnAfterRenderAsync(firstRender); //Set the proper Id
		}

		void IDisposable.Dispose()
		{
			JsService.RemoveCKEditor(Id);
			if (_objectReference != null)
			{
				_objectReference.Dispose();
				_objectReference = null;
			}
		}

		protected override async Task OnInitializedAsync()
		{
			if (!String.IsNullOrWhiteSpace(Class))
				_cssList.Add(Class);

			var sizeSuffix = Size.ToDescriptionString();
			if (!String.IsNullOrWhiteSpace(sizeSuffix))
				_cssList.Add($"form-control-{sizeSuffix}");


			await base.OnInitializedAsync();
		}

		protected override async Task OnParametersSetAsync()
		{
			if (JsonConvert.SerializeObject(_originalValue) != JsonConvert.SerializeObject(Value))
			{
				_originalValue = Value;
				_value = FieldValueService.InitAsString(Value);
				await JsService.SetCKEditorData(Id,_value);
			}

			if (!String.IsNullOrWhiteSpace(Name))
				AdditionalAttributes["name"] = Name;

			await base.OnParametersSetAsync();
		}
		#endregion

		#region << Private methods >>


		#endregion

		#region << Ui handlers >>
		private async Task _onKeyDownHandler(KeyboardEventArgs e)
		{
			//Needs to be keydown as keypress is produced only on printable chars (does not work on backspace
			await Task.Delay(5);
			if (e.Key == "Tab")
			{
				await ValueChanged.InvokeAsync(new ChangeEventArgs { Value = _value });
			}

			await OnKeyDown.InvokeAsync(e);
			await OnInput.InvokeAsync(new ChangeEventArgs { Value = _value });
		}

		private async Task _onBlurHandler(FocusEventArgs e)
		{
			await ValueChanged.InvokeAsync(new ChangeEventArgs { Value = _value });
			await OnInput.InvokeAsync(new ChangeEventArgs { Value = _value });
		}

		#endregion

		#region << JS Callbacks methods >>
		[JSInvokable]
		public async Task NotifyChange(string content)
		{
			await ValueChanged.InvokeAsync(new ChangeEventArgs { Value = content });
			await OnInput.InvokeAsync(new ChangeEventArgs { Value = content });

		}
		#endregion
	}
}