using Mina.Core.Session;
using Mina.Filter.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket.Codec
{
    public class DefaultPacketFactory : IProtocolCodecFactory
    {
        private IProtocolEncoder _Encoder;
        private IProtocolDecoder _Decoder;

        public DefaultPacketFactory()
        {
            this._Encoder = new DefaultPacketEncoder();
            this._Decoder = new DefaultPacketDecoder();
        }

        public IProtocolDecoder GetDecoder(IoSession session)
        {
            return this._Decoder;
        }

        public IProtocolEncoder GetEncoder(IoSession session)
        {
            return this._Encoder;
        }
    }
}
