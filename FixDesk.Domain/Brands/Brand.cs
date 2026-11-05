using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Brands
{
    // Lookup: الاسم فقط، IsActive بدل الحذف، وUnique على الاسم (يُفرض في الـ Infrastructure).
    public class Brand : AggregateRoot<BrandId>
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        [SetsRequiredMembers]
        private Brand(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name cannot be null or empty.", nameof(name));

            Id = BrandId.New();
            Name = name.Trim();
            IsActive = true;
        }

        public static Brand Create(string name)
        {
            return new Brand(name);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Brand name cannot be null or empty.", nameof(name));
            Name = name.Trim();
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
