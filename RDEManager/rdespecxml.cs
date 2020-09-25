using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace RDEManager
{
    public class XMLSpecimenList
    {
        public XMLSpecimenList()
        {

        }

        public XMLSpecimen[] Specimens { get; set; }
        
        
        public XMLSpecimenList(string rdespec)
        {
            rdespec = rdespec.Trim();
            if (string.IsNullOrEmpty(rdespec))
            {
                throw new Exception("specimen xml cannot be empty");
            }

            //wrap it in a containter
            rdespec = $"<SpecimenList>{rdespec}</SpecimenList>".Replace("&", "&amp;");
            try
            {
                using (StringReader reader = new StringReader(rdespec))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<XMLSpecimen>), new XmlRootAttribute("SpecimenList"));
                    List<XMLSpecimen> list = (List<XMLSpecimen>)serializer.Deserialize(reader);
                    this.Specimens = list.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

                    xml = xml.Replace("<SpecimenList>", "").Replace("</SpecimenList>", "").Replace("&amp;", "&");

                    return xml;

                }
            }
        }

    }

    [XmlType("specimen")]
    public class XMLSpecimen
    {
        public string ih { get; set; }
        public string ccid { get; set; }
        public string barcode { get; set; }
        public string oldbarcode { get; set; }
        public string accession { get; set; }
        public string genbank { get; set; }
        public string herblocate { get; set; }
        public string type { get; set; }
        public string phenology { get; set; }
        public string hstype { get; set; }
        public string tfamily { get; set; }
        public string tgenus { get; set; }
        public string tsp1 { get; set; }
        public string tau1 { get; set; }
        public string trank1 { get; set; }
        public string tsp2 { get; set; }
        public string tau2 { get; set; }
        public string trank2 { get; set; }
        public string tsp3 { get; set; }
        public string tau3 { get; set; }
        public string trank3 { get; set; }
        public string tunique { get; set; }
        public string tproto { get; set; }
        public string tprotoyear { get; set; }
        public string imagelink { get; set; }
        public string sptype { get; set; }
        public string seenby { get; set; }
        public string note { get; set; }
        public string seenwhere { get; set; }
    }
}
