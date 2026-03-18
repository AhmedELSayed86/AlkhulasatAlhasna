using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Alkhulasat.BusinessLogic.Messages
{
    public class SettingsChangedMessage : ValueChangedMessage<string>
    {
        public SettingsChangedMessage(string value) : base(value) { }
    }
}
