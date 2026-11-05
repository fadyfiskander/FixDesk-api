using FixDesk.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.RepairTickets
{
    // كائن القيمة لحالة الجهاز عند الاستلام
    public class DeviceCondition : ValueObject
    {
        public string PhysicalStatus { get; }
        public string DefectDescription { get; }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return PhysicalStatus;
            yield return DefectDescription;
        }

        private DeviceCondition(string physicalStatus, string defectDescription)
        {
            PhysicalStatus = physicalStatus;
            DefectDescription = defectDescription;
        }

        public static DeviceCondition Create(string physicalStatus, string defectDescription)
        {
            if (string.IsNullOrEmpty(physicalStatus))
                throw new ArgumentException("Physical status cannot be null or empty.", nameof(physicalStatus));
            if (string.IsNullOrEmpty(defectDescription))
                throw new ArgumentException("Defect description cannot be null or empty.", nameof(defectDescription));
            return new DeviceCondition(physicalStatus, defectDescription);
        }

        public DeviceCondition UpdatePhysicalStatus(string physicalStatus)
        {
            if (string.IsNullOrEmpty(physicalStatus))
                throw new ArgumentException("Physical status cannot be null or empty.", nameof(physicalStatus));
            return new DeviceCondition(physicalStatus, DefectDescription);
        }

        public DeviceCondition UpdateDefectDescription(string defectDescription)
        {
            if (string.IsNullOrEmpty(defectDescription))
                throw new ArgumentException("Defect description cannot be null or empty.", nameof(defectDescription));
            return new DeviceCondition(PhysicalStatus, defectDescription);
        }
    }
}