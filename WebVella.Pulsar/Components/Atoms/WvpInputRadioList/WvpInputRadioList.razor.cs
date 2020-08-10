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
	public partial class WvpInputRadioList<TItem> : WvpInputBase
	{

		#region << Parameters >>

		[Parameter] public bool IsVertical { get; set; } = false;

		[Parameter] public RenderFragment<TItem> WvpInputRadioListOption { get; set; }

		[Parameter] public IEnumerable<TItem> Options { get { return _options; } set { _options = value; _isDataTouched = true; } }

		[Parameter] public TItem Value { get; set; }

		#endregion

		#region << Callbacks >>

		#endregion

		#region << Private properties >>

		private List<string> _cssList = new List<string>();

		private IEnumerable<TItem> _options;

		private string _groupName = "wvp-group-" + Guid.NewGuid();

		private TItem _originalValue;

		private TItem _value;

		private bool _isDataTouched = true;

		#endregion

		#region << Lifecycle methods >>

		protected override async Task OnParametersSetAsync()
		{
			_cssList = new List<string>();

			if (!String.IsNullOrWhiteSpace(Class))
				_cssList.Add(Class);

			var sizeSuffix = Size.ToDescriptionString();
			if (!String.IsNullOrWhiteSpace(sizeSuffix))
				_cssList.Add($"form-control-{sizeSuffix}");


			if (JsonConvert.SerializeObject(_originalValue) != JsonConvert.SerializeObject(Value))
			{
				_originalValue = Value;
				_value = JsonConvert.DeserializeObject<TItem>(JsonConvert.SerializeObject(Value));
			}

			if (!String.IsNullOrWhiteSpace(Name))
				_groupName = Name;

			await base.OnParametersSetAsync();
		}
		#endregion

		#region << Private methods >>


		#endregion

		#region << Ui handlers >>

		private async Task _onInputHandler(ChangeEventArgs e)
		{
			await OnInput.InvokeAsync(e);
		}

		private async Task _onSelectHandler(TItem item)
		{
			_value = item;

			await OnInput.InvokeAsync(new ChangeEventArgs { Value = _value });
			await ValueChanged.InvokeAsync(new ChangeEventArgs { Value = _value });
		}

		#endregion

		#region << JS Callbacks methods >>

		#endregion
	}
}