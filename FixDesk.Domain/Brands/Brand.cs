using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Brands
{
    public class Brand : AggregateRoot<BrandId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        [SetsRequiredMembers]
        private Brand(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Brand name cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Brand description cannot be null or empty.", nameof(description));
            Id = BrandId.New();
            Name = name;
            Description = description;
            IsActive = true;
        }

        public static Brand Create(string name, string description)
        {
            return new Brand(name, description);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Brand name cannot be null or empty.", nameof(name));
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Brand description cannot be null or empty.", nameof(description));
            Description = description;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}