using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FixDesk.Domain.RepairTickets
{
    public class RepairTicket : AggregateRoot<RepairTicketId>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public RepairTicketState TicketState { get; private set; }
        public CustomerId CustomerId { get; private set; }

        public DeviceCondition ReceptionCondition { get; private set; }

        private readonly List<DeviceAttachment> _attachments = new();
        public IReadOnlyCollection<DeviceAttachment> Attachments => _attachments.AsReadOnly();

        [SetsRequiredMembers]
        private RepairTicket(string title, string description, CustomerId customerId, DeviceCondition receptionCondition)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Repair ticket title cannot be null or empty.", nameof(title));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Repair ticket description cannot be null or empty.", nameof(description));

            Id = RepairTicketId.New();
            Title = title;
            Description = description;
            TicketState = RepairTicketState.Received;
            CustomerId = customerId;
            ReceptionCondition = receptionCondition;
        }

        public static RepairTicket Create(string title, string description, CustomerId customerId, DeviceCondition receptionCondition)
        {
            return new RepairTicket(title, description, customerId, receptionCondition);
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Repair ticket title cannot be null or empty.", nameof(title));
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Repair ticket description cannot be null or empty.", nameof(description));
            Description = description;
        }

        #region State Management
        public void AssignAwaitingApproval()
        {
            TicketState = RepairTicketState.AwaitingApproval;
        }

        public void AssignInRepair()
        {
            TicketState = RepairTicketState.InRepair;
        }

        public void AssignReady()
        {
            TicketState = RepairTicketState.Ready;
        }

        public void AssignDelivered()
        {
            TicketState = RepairTicketState.Delivered;
        }

        public void AssignRejected()
        {
            TicketState = RepairTicketState.Rejected;
        }

        public void AssignCancelled()
        {
            TicketState = RepairTicketState.Cancelled;
        }
        #endregion

        public void UpdateReceptionCondition(string PhysicalStatus, string DefectDescription)
        {
            var updatedCondition = DeviceCondition.Create(PhysicalStatus, DefectDescription);
            if(!ReceptionCondition.Equals(updatedCondition)) return;
            ReceptionCondition = updatedCondition;
        }

        public void AddAttachment(DeviceAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null.");
            _attachments.Add(attachment);
        }

        public void RemoveAttachment(DeviceAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "Attachment cannot be null.");
            _attachments.Remove(attachment);
        }
    }
}