namespace Domain.Core.Tests.Financial.Cfis
{
    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Cfis;

    using NUnit.Framework;

    [TestFixture]
    public class CfiInstrumentTypeMapperTests
    {
        [Test]
        public void MapCfi_Null_Is_None()
        {
            var mapper = new CfiInstrumentTypeMapper();

            var result = mapper.MapCfi(null);

            Assert.AreEqual(result, InstrumentTypes.None);
        }

        [TestCase("db")]
        [TestCase("dB")]
        [TestCase("dbe-1")]
        [TestCase("dbagequi")]
        [TestCase("dbd")]
        [TestCase("dbebamneb aebi=04i3 3kh nm aet[k ")]
        [TestCase("db eaepgoearpgjoreag")]
        public void MapCfi_StartsWithDb_IsBond(string bondStr)
        {
            var mapper = this.GetMapper();

            var result = mapper.MapCfi(bondStr);

            Assert.AreEqual(result, InstrumentTypes.Bond);
        }

        [TestCase("e")]
        [TestCase("E")]
        [TestCase("e-1")]
        [TestCase("equi")]
        [TestCase("ee")]
        [TestCase("ebamneb aebi=04i3 3kh nm aet[k ")]
        [TestCase("eaepgoearpgjoreag")]
        public void MapCfi_StartsWithE_IsEquity(string equityStr)
        {
            var mapper = this.GetMapper();

            var result = mapper.MapCfi(equityStr);

            Assert.AreEqual(result, InstrumentTypes.Equity);
        }

        [TestCase("oc")]
        [TestCase("OC")]
        [TestCase("oCe-1")]
        [TestCase("ocption")]
        [TestCase("oCoi")]
        [TestCase("ocebamneb aebi=04i3 3kh nm aet[k ")]
        [TestCase("oc eaepgoearpgjoreag")]
        public void MapCfi_StartsWithOc_IsOptionCall(string optionCallStr)
        {
            var mapper = this.GetMapper();

            var result = mapper.MapCfi(optionCallStr);

            Assert.AreEqual(result, InstrumentTypes.OptionCall);
        }

        [TestCase("op")]
        [TestCase("OP")]
        [TestCase("oPe-1")]
        [TestCase("OPelequi")]
        [TestCase("oPPP")]
        [TestCase("opeamneb aebi=04i3 3kh nm aet[k ")]
        [TestCase("Opeaepgoearpgjoreag")]
        public void MapCfi_StartsWithOp_IsOptionPut(string optionPutStr)
        {
            var mapper = this.GetMapper();

            var result = mapper.MapCfi(optionPutStr);

            Assert.AreEqual(result, InstrumentTypes.OptionPut);
        }

        [TestCase("aergeag")]
        [TestCase("drn")]
        [TestCase("249tjh")]
        [TestCase("£2p4oi24jh")]
        public void MapCfi_StartsWithWhatever_IsUnknown(string unknownStr)
        {
            var mapper = this.GetMapper();

            var result = mapper.MapCfi(unknownStr);

            Assert.AreEqual(result, InstrumentTypes.Unknown);
        }

        /// <summary>
        ///     centralise use of ctor for type
        /// </summary>
        private CfiInstrumentTypeMapper GetMapper()
        {
            return new CfiInstrumentTypeMapper();
        }
    }
}