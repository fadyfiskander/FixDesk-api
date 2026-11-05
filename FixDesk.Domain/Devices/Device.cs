using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Brands;
using FixDesk.Domain.Categories;
using FixDesk.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FixDesk.Domain.Devices
{
    public class Device : AggregateRoot<DeviceId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string? SerialNumber { get; private set; }
        public string Model { get; private set; }
        public BrandId BrandId { get; private set; }
        public CategoryId CategoryId { get; private set; }

        [SetsRequiredMembers]
        private Device(string name, string description, string model, BrandId brandId, CategoryId categoryId, string? serialNumber = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Device name cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Device description cannot be null or empty.", nameof(description));
            if(string.IsNullOrEmpty(model))
                throw new ArgumentException("Device model cannot be null or empty.", nameof(model));

            Id = DeviceId.New();
            Name = name;
            Description = description;
            Model = model;
            BrandId = brandId;
            CategoryId = categoryId;
            SerialNumber = serialNumber;
        }

        public static Device Create(string name, string description, string model, BrandId brandId, CategoryId categoryId, string? serialNumber = null)
        {
            return new Device(name, description, model, brandId, categoryId, serialNumber);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Device name cannot be null or empty.", nameof(name));
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Device description cannot be null or empty.", nameof(description));
            Description = description;
        }

        public void UpdateModel(string model)
        {
            if (string.IsNullOrEmpty(model))
                throw new ArgumentException("Device model cannot be null or empty.", nameof(model));
            Model = model;
        }

        public void UpdateSerialNumber(string? serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void UpdateBrand(BrandId brandId)
        {
            BrandId = brandId;
        }

        public void UpdateCategory(CategoryId categoryId)
        {
            CategoryId = categoryId;
        }

        public void UpdateDevice(string name, string description, BrandId brandId, CategoryId categoryId, string? serialNumber = null)
        {
            Rename(name);
            UpdateDescription(description);
            UpdateBrand(brandId);
            UpdateCategory(categoryId);
            UpdateSerialNumber(serialNumber);
        }
    }
}