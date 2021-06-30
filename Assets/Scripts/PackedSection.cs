using System;

namespace FilePacker
{
    public enum SectionType { Null, String, Bytes, Int, Float }

    public interface PackedSection
    {
        public Type Type { get; }
    }

    public abstract class TypeSection<T> : PackedSection
    {
        Type PackedSection.Type => typeof(T);
        public T Value;

        public TypeSection(T value)
        {
            Value = value;
        }
    }

    public class StringSection : TypeSection<string>
    {
        public StringSection(string value) : base(value)
        {
        }
    }

    public class BytesSection : TypeSection<byte[]>
    {
        public BytesSection(byte[] value) : base(value)
        {
        }
    }
}
