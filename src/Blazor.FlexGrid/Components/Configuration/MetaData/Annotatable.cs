using System;
using System.Collections.Generic;

namespace Blazor.FlexGrid.Components.Configuration.MetaData
{
    public class Annotatable : IAnnotatable
    {
        private readonly SortedDictionary<string, Annotation> _annotations;

        public object this[string name]
        {
            get => FindAnnotation(name).Value;
            set
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }

                if (value is null)
                {
                    RemoveAnnotation(name);
                }
                else
                {
                    SetAnnotation(name, value);
                }
            }

        }

        public Annotatable()
        {
            _annotations = new SortedDictionary<string, Annotation>();
        }

        public Annotation FindAnnotation(string name)
            => _annotations.TryGetValue(name, out var annotation)
                    ? annotation
                    : NullAnnotation.Instance;


        public IEnumerable<IAnnotation> GetAllAnnotations()
            => _annotations.Values;

        public Annotation SetAnnotation(string name, object value)
        {
            var annotation = CreateAnnotation(name, value);

            _annotations[name] = annotation;

            return annotation;
        }

        public Annotation RemoveAnnotation(string name)
        {
            var annotation = FindAnnotation(name);
            if (annotation is NullAnnotation)
            {
                return NullAnnotation.Instance;
            }

            _annotations.Remove(name);

            return annotation;
        }

        IAnnotation IAnnotatable.FindAnnotation(string name)
            => FindAnnotation(name);

        protected virtual Annotation CreateAnnotation(string name, object value)
            => new Annotation(name, value);
    }
}
