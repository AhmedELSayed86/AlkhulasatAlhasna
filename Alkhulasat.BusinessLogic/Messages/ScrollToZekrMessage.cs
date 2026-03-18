using Alkhulasat.Domain.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Alkhulasat.BusinessLogic.Messages
{
    public class ScrollToZekrMessage : ValueChangedMessage<ZekrModel>
    {
        public ScrollToZekrMessage(ZekrModel value) : base(value) { }
    }

}
