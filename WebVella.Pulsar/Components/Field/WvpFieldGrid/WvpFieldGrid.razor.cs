﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using WebVella.Pulsar.Models;
using WebVella.Pulsar.Services;
using WebVella.Pulsar.Utils;


namespace WebVella.Pulsar.Components
{
	public partial class WvpFieldGrid<ValueItemType> : WvpFieldBase, IDisposable
	{

		#region << Parameters >>
		[Parameter] public GridMode GridMode { get; set; } = GridMode.Table;
		[Parameter] public bool ShowColumnHeader { get; set; } = true;
		[Parameter] public bool ShowColumnFooter { get; set; } = false;
		[Parameter] public bool Paging { get; set; } = true;
		[Parameter] public int PageSize { get; set; } = 10;
		[Parameter] public int CurrentPage { get; set; } = 0;
		[Parameter] public int TotalCount { get; set; } = 0;
		[Parameter] public RenderFragment HeaderTemplate { get; set; }
		[Parameter] public RenderFragment FooterTemplate { get; set; }
		[Parameter] public List<ValueItemType> GenericOptions { get; set; } = new List<ValueItemType>();
		[Parameter] public List<GridActions> Actions { get; set; } = new List<GridActions>() { GridActions.Insert, GridActions.Update, GridActions.Delete };
		[Parameter] public List<WvpFieldGridColumn<object>> GridColumns { get; set; } = new List<WvpFieldGridColumn<object>>();
		[Parameter] public EventCallback<ChangeEventArgs> PageChanged { get; set; }
		[Parameter] public EventCallback<ChangeEventArgs> PageSizeChanged { get; set; }
		[Parameter] public RenderFragment<List<WvpFieldGridColumn<object>>> ColumnsTemplate { get; set; }
		[Parameter] public RenderFragment InsertTemplate { get; set; }
		[Parameter] public RenderFragment UpdateTemplate { get; set; }
		[Parameter] public EventCallback<ValueItemType> InsertHandler { get; set; }
		[Parameter] public EventCallback<ValueItemType> UpdateHandler { get; set; }
		[Parameter] public EventCallback<ValueItemType> DeleteHandler { get; set; }

		#endregion

		#region << Callbacks >>

		#endregion

		#region << Private properties >>

		private string _domElementId = "";

		private DotNetObjectReference<WvpFieldGrid<ValueItemType>> _objectReference;

		private bool _editEnabled = false;

		private Guid _originalFieldId;

		private object _originalValue;

		private ValueItemType _defaultValue = default(ValueItemType);
		private ValueItemType _value = default(ValueItemType);

		private List<ValueItemType> _originalOptions = new List<ValueItemType>();
		private List<ValueItemType> _options = new List<ValueItemType>();

		#endregion

		#region << Lifecycle methods >>
		void IDisposable.Dispose()
		{
			new JsService(JSRuntime).RemoveDocumentEventListener(WvpDomEventType.KeydownEscape, FieldId.ToString());
			new JsService(JSRuntime).RemoveOutsideClickEventListener($"#{_domElementId}", FieldId.ToString());

			if (_objectReference != null)
			{
				_objectReference.Dispose();
				_objectReference = null;
			}
		}

		protected override async Task OnParametersSetAsync()
		{
			if (JsonConvert.SerializeObject(_originalValue) != JsonConvert.SerializeObject(Value))
			{
				_originalValue = Value;
				if (Value != null)
				{
					if (Value.GetType() != typeof(ValueItemType))
					{
						throw new Exception("Invalid value type");
					}
					else
						_value = FieldValueService.InitAsGeneric<ValueItemType>(Value);
				}
			}
			if (_originalFieldId != FieldId)
			{
				_originalFieldId = FieldId;
				_domElementId = "wvp-field-grid-" + FieldId;
			}

			if ((_options == null || _options.Count == 0 || JsonConvert.SerializeObject(_originalOptions) != JsonConvert.SerializeObject(GenericOptions)))
			{
				_originalOptions = GenericOptions;
				_options = GenericOptions;
			}

			await base.OnParametersSetAsync();
		}

		#endregion

		#region << Private methods >>

		#endregion

		#region << UI handlers >>
		private void _toggleInlineEditClickHandler(bool enableEdit, bool applyChange)
		{

			//Show Edit
			if (enableEdit && !_editEnabled)
			{
				_editEnabled = true;
				new JsService(JSRuntime).AddDocumentEventListener(WvpDomEventType.KeydownEscape, _objectReference, FieldId.ToString(), "OnEscapeKey");
				new JsService(JSRuntime).AddOutsideClickEventListener($"#{_domElementId}", _objectReference, FieldId.ToString(), "OnFocusOutClick");
			}

			//Hide edit
			if (!enableEdit && _editEnabled)
			{
				var originalValue = FieldValueService.InitAsGeneric<object>(Value);
				//Apply Change
				if (applyChange)
				{
					if (JsonConvert.SerializeObject(_value) != JsonConvert.SerializeObject(originalValue))
					{
						//Update Function should be called
						ValueChanged.InvokeAsync(new ChangeEventArgs() { Value = _value });
					}
				}
				//Abandon change
				else
				{
					_value = (ValueItemType)_originalValue;
				}
				_editEnabled = false;
				new JsService(JSRuntime).RemoveDocumentEventListener(WvpDomEventType.KeydownEscape, FieldId.ToString());
				new JsService(JSRuntime).RemoveOutsideClickEventListener($"#{_domElementId}", FieldId.ToString());
			}

		}

		private void _onInsertHandler(ValueItemType item)
		{
			if (InsertHandler.HasDelegate)
				InsertHandler.InvokeAsync(item);
		}

		private void _onUpdateHandler(ValueItemType item)
		{
			if (UpdateHandler.HasDelegate)
				UpdateHandler.InvokeAsync(item);
		}

		private void _onDeleteHandler(ValueItemType item)
		{
			if (DeleteHandler.HasDelegate)
				DeleteHandler.InvokeAsync(item);
		}
		#endregion

		#region << JS Callbacks methods >>

		[JSInvokable]
		public async Task OnEscapeKey()
		{
			if (_editEnabled)
			{
				_toggleInlineEditClickHandler(false, false);
				StateHasChanged();
			}
		}

		[JSInvokable]
		public async Task OnFocusOutClick()
		{
			if (_editEnabled)
			{
				_toggleInlineEditClickHandler(false, true);
				StateHasChanged();
			}
		}
		#endregion

	}

	public enum GridMode
	{
		Table,
		BootstrapGrid
	}

	public enum GridActions
	{
		Insert,
		Update,
		Delete
	}
}