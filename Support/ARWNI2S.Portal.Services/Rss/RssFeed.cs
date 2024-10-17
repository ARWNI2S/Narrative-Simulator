using System.Xml;
using System.Xml.Linq;

namespace ARWNI2S.Portal.Services.Rss
{
    /// <summary>
    /// Represents the RSS feed
    /// </summary>
    public partial class RssFeed
    {
        #region Ctor

        /// <summary>
        /// Initialize new instance of RSS feed
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="link">Link</param>
        /// <param name="lastBuildDate">Last build date</param>
        public RssFeed(string title, string description, Uri link, DateTimeOffset lastBuildDate)
        {
            Title = new XElement(RssDefaults.Title, title);
            Description = new XElement(RssDefaults.Description, description);
            Link = new XElement(RssDefaults.Link, link);
            LastBuildDate = new XElement(RssDefaults.LastBuildDate, lastBuildDate.ToString("r"));
        }

        /// <summary>
        /// Initialize new instance of RSS feed
        /// </summary>
        /// <param name="link">URL</param>
        public RssFeed(Uri link) : this(string.Empty, string.Empty, link, DateTimeOffset.Now)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Attribute extension
        /// </summary>
        public KeyValuePair<XmlQualifiedName, string> AttributeExtension { get; set; }

        /// <summary>
        /// Element extensions
        /// </summary>
        public List<XElement> ElementExtensions { get; } = [];

        /// <summary>
        /// List of rss items
        /// </summary>
        public List<RssItem> Items { get; set; } = [];

        /// <summary>
        /// Title
        /// </summary>
        public XElement Title { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public XElement Description { get; private set; }

        /// <summary>
        /// Link
        /// </summary>
        public XElement Link { get; private set; }

        /// <summary>
        /// Last build date
        /// </summary>
        public XElement LastBuildDate { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Load RSS feed from the passed stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the RSS feed
        /// </returns>
        public static async Task<RssFeed> LoadAsync(Stream stream)
        {
            try
            {
                var document = await XDocument.LoadAsync(stream, LoadOptions.None, default);

                var channel = document.Root?.Element(RssDefaults.Channel);

                if (channel == null)
                    return null;

                var title = channel.Element(RssDefaults.Title)?.Value ?? string.Empty;
                var description = channel.Element(RssDefaults.Description)?.Value ?? string.Empty;
                var link = new Uri(channel.Element(RssDefaults.Link)?.Value ?? string.Empty);
                var lastBuildDateValue = channel.Element(RssDefaults.LastBuildDate)?.Value;
                var lastBuildDate = lastBuildDateValue == null ? DateTimeOffset.Now : DateTimeOffset.ParseExact(lastBuildDateValue, "r", null);

                var feed = new RssFeed(title, description, link, lastBuildDate);

                foreach (var item in channel.Elements(RssDefaults.Item))
                {
                    feed.Items.Add(new RssItem(item));
                }

                return feed;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get content of this RSS feed
        /// </summary>
        /// <returns>Content of RSS feed</returns>
        public string GetContent()
        {
            var document = new XDocument();
            var root = new XElement(RssDefaults.RSS, new XAttribute("version", "2.0"));
            var channel = new XElement(RssDefaults.Channel,
                new XAttribute(XName.Get(AttributeExtension.Key.Name, AttributeExtension.Key.Namespace), AttributeExtension.Value));

            channel.Add(Title, Description, Link, LastBuildDate);

            foreach (var element in ElementExtensions)
            {
                channel.Add(element);
            }

            foreach (var item in Items)
            {
                channel.Add(item.ToXElement());
            }

            root.Add(channel);
            document.Add(root);

            return document.ToString();
        }

        #endregion
    }
}