using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RDEManager
{
    [XmlRoot("SpecimenList")]
    public class XMLSpecimenList
    {
        public XMLSpecimenList()
        {

        }

        public XMLSpecimenList(string rdespec)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLSpecimenList));

            //we need to add the list wrapper
            rdespec = $"<SpecimenList>{rdespec}</SpecimenList>";

            using (TextReader reader = new StringReader(rdespec))
            {
                XMLSpecimenList newList = (XMLSpecimenList)serializer.Deserialize(reader);
                this.Specimens = newList.Specimens;
            }
        }

        public string ToXMLString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLSpecimenList));
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, this);
                    string xml = sww.ToString(); // Your XML

                    xml = xml.Replace("<SpecimenList>", "").Replace("</SpecimenList>", "");

                    return xml;

                }
            }
        }

        [XmlElement("Specimen")]
        public List<XMLSpecimen> Specimens { get; set; }
    }

    public class XMLSpecimen
    {
        public XMLSpecimen()
        {

        }

        [XmlElement("ih")]
        public string ih { get; set; }

        [XmlElement("ccid")]
        public string ccid { get; set; }

        [XmlElement("barcode")]
        public string barcode { get; set; }

        [XmlElement("oldbarcode")]
        public string oldbarcode { get; set; }

        [XmlElement("accession")]
        public string accession { get; set; }

        [XmlElement("genbank")]
        public string genbank { get; set; }

        [XmlElement("herblocate")]
        public string herblocate { get; set; }

        [XmlElement("type")]
        public string type { get; set; }

        [XmlElement("phenology")]
        public string phenology { get; set; }

        [XmlElement("hstype")]
        public string hstype { get; set; }

        [XmlElement("tfamily")]
        public string tfamily { get; set; }

        [XmlElement("tgenus")]
        public string tgenus { get; set; }

        [XmlElement("tsp1")]
        public string tsp1 { get; set; }

        [XmlElement("tau1")]
        public string tau1 { get; set; }

        [XmlElement("trank1")]
        public string trank1 { get; set; }

        [XmlElement("tsp2")]
        public string tsp2 { get; set; }

        [XmlElement("tau2")]
        public string tau2 { get; set; }

        [XmlElement("trank2")]
        public string trank2 { get; set; }

        [XmlElement("tsp3")]
        public string tsp3 { get; set; }

        [XmlElement("tau3")]
        public string tau3 { get; set; }

        [XmlElement("trank3")]
        public string trank3 { get; set; }

        [XmlElement("tunique")]
        public string tunique { get; set; }

        [XmlElement("tproto")]
        public string tproto { get; set; }

        [XmlElement("tprotoyear")]
        public string tprotoyear { get; set; }

        [XmlElement("imagelink")]
        public string imagelink { get; set; }

        [XmlElement("sptype")]
        public string sptype { get; set; }

        [XmlElement("seenby")]
        public string seenby { get; set; }

        [XmlElement("note")]
        public string note { get; set; }

        [XmlElement("seenwhere")]
        public string seenwhere { get; set; }

        [XmlElement("dethist")]
        public string dethist { get; set; }

    }
}
