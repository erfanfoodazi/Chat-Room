namespace Domain.Entities;
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentTime { get; set; } = DateTime.UtcNow;

        public bool IsDelivered { get; set; }
        public bool IsSeen { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? DeliveredTime { get; set; }
        public DateTime? SeenTime { get; set; }
        public DateTime? EditedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        public int SenderId { get; set; }
        public virtual User Sender { get; set; } = null!;

        public int? ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }

        public int? PersonalChatId { get; set; }
        public virtual PersonalChat? PersonalChat { get; set; }

        public int? GroupChatId { get; set; }
        public virtual GroupChat? GroupChat { get; set; }

        public int? ReplyToMessageId { get; set; }
        public virtual Message? ReplyTo { get; set; }

        public bool IsPersonalMessage => PersonalChatId.HasValue;
        public bool IsGroupMessage => GroupChatId.HasValue;


        public void MarkAsDelivered()
        {
            IsDelivered = true;
            DeliveredTime = DateTime.UtcNow;
        }

        public void MarkAsSeen()
        {
            IsSeen = true;
            SeenTime = DateTime.UtcNow;
        }

        public void Edit(string newText)
        {
            Text = newText;
            IsEdited = true;
            EditedTime = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedTime = DateTime.UtcNow;
            Text = "This message was deleted";
        }

        public string GetDisplayStatus()
        {
            if (IsDeleted) return "Deleted";
            if (IsSeen) return "Seen";
            if (IsDelivered) return "Delivered";
            if (IsEdited) return "Edited";
            return "Sent";
        }


    }

