using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Proxy
{
    [ContentProperty(nameof(Bindings))]
    public class MultiBinding : BindingBase
    {
        private readonly BindingExpression _bindingExpression;
        private readonly InternalValue _internalValue = new InternalValue();

        private bool _isApplying;
        private IMultiValueConverter _converter;
        private object _converterParameter;

        private BindableProperty[] Properties { get; set; }

        public IList<BindingBase> Bindings { get; } = new List<BindingBase>();

        public IMultiValueConverter Converter
        {
            get => _converter;
            set
            {
                ThrowIfApplied();
                _converter = value;
            }
        }

        public object ConverterParameter
        {
            get => _converterParameter;
            set
            {
                ThrowIfApplied();
                _converterParameter = value;
            }
        }

        public MultiBinding()
        {
            Mode = BindingMode.OneWay;
            _bindingExpression = new BindingExpression(this, nameof(InternalValue.Value));
        }

        internal override void Apply(object context, BindableObject bindObj, BindableProperty targetProperty,
            bool fromBindingContextChanged = false)
        {
            if (Mode != BindingMode.OneWay)
                throw new InvalidOperationException($"{nameof(MultiBinding)} only supports {nameof(Mode)}.{nameof(BindingMode.OneWay)}");

            base.Apply(context, bindObj, targetProperty, fromBindingContextChanged);

            _isApplying = true;
            Properties = new BindableProperty[Bindings.Count];
            int i = 0;
            foreach (BindingBase binding in Bindings)
            {
                var property = BindableProperty.Create($"{nameof(MultiBinding)}Property-{Guid.NewGuid():N}", typeof(object),
                    typeof(MultiBinding), default(object), propertyChanged: (bindableObj, o, n) =>
                    {
                        SetInternalValue(bindableObj);
                    });
                Properties[i++] = property;
                bindObj.SetBinding(property, binding);
            }
            _isApplying = false;
            SetInternalValue(bindObj);

            _bindingExpression.Apply(_internalValue, bindObj, targetProperty);
        }

        internal override void Apply(bool fromTarget)
        {
            base.Apply(fromTarget);
            foreach (BindingBase binding in Bindings)
            {
                binding.Apply(fromTarget);
            }
            _bindingExpression.Apply(fromTarget);
        }

        internal override void Unapply(bool fromBindingContextChanged = false)
        {
            base.Unapply(fromBindingContextChanged);
            foreach (BindingBase binding in Bindings)
            {
                binding.Unapply(fromBindingContextChanged);
            }
            Properties = null;
            _bindingExpression?.Unapply();
        }

        internal override object GetSourceValue(object value, Type targetPropertyType)
        {
            if (Converter != null)
                value = Converter.Convert(value as object[], targetPropertyType, ConverterParameter, CultureInfo.CurrentUICulture);
            if (StringFormat != null && value != null)
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (value is object[] array)
                {
                    value = string.Format(StringFormat, array);
                }
                else
                {
                    value = string.Format(StringFormat, value);
                }
            }
            return value;
        }

        internal override object GetTargetValue(object value, Type sourcePropertyType)
        {
            throw new InvalidOperationException($"{nameof(MultiBinding)} only supports {nameof(Mode)}.{nameof(BindingMode.OneWay)}");
        }

        private void SetInternalValue(BindableObject source)
        {
            if (source == null || _isApplying) return;
            _internalValue.Value = Properties.Select(source.GetValue).ToArray();
        }

        internal override BindingBase Clone()
        {
            var rv = new MultiBinding
            {
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                StringFormat = StringFormat
            };
            rv._internalValue.Value = _internalValue.Value;

            foreach (var binding in Bindings.Select(x => x.Clone()))
            {
                rv.Bindings.Add(binding);
            }
            return rv;
        }

        private sealed class InternalValue : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private object _value;
            public object Value
            {
                get => _value;
                set
                {
                    if (!Equals(_value, value))
                    {
                        _value = value;
                        OnPropertyChanged();
                    }
                }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
