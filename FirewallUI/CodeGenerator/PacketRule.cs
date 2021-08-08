namespace CodeManager
{
    public class PacketRule
    {
        public bool AllowIncoming {  get; private set; }

        public bool AllowOutgoing { get; private set; }

        public PacketRule(bool incoming, bool outgoing)
        {
            AllowIncoming = incoming;
            AllowOutgoing = outgoing;
        }

        public override string ToString()
        {
            return $"({AllowIncoming.ToString().ToLower()},{AllowOutgoing.ToString().ToLower()})";
        }
    }
}
