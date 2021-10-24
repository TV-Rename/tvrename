using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public class Regions : List<Region>
    {
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile Regions? InternalInstance;
        private static readonly object SyncRoot = new();

        public static Regions Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                        if (InternalInstance is null)
                        {
                            InternalInstance = new Regions();
                        }
                    }
                }

                return InternalInstance;
            }
        }

        public IEnumerable<string> EnglishNames => this.Select(r => r.EnglishName);

        public Region FallbackRegion => RegionFromCode("US")!;

        private Regions()
        {
            Add(new Region(1, "AD", "AND", string.Empty, "Andorra"));
            Add(new Region(2, "AE", "ARE", string.Empty, "United Arab Emirates"));
            Add(new Region(3, "AF", "AFG", string.Empty, "Afghanistan"));
            Add(new Region(4, "AG", "ATG", string.Empty, "Antigua and Barbuda"));
            Add(new Region(5, "AI", "AIA", string.Empty, "Anguilla"));
            Add(new Region(6, "AL", "ALB", string.Empty, "Albania"));
            Add(new Region(7, "AM", "ARM", string.Empty, "Armenia"));
            Add(new Region(8, "AO", "AGO", string.Empty, "Angola"));
            Add(new Region(9, "AQ", "ATA", string.Empty, "Antarctica"));
            Add(new Region(10, "AR", "ARG", string.Empty, "Argentina"));
            Add(new Region(11, "AS", "ASM", string.Empty, "American Samoa"));
            Add(new Region(12, "AT", "AUT", string.Empty, "Austria"));
            Add(new Region(13, "AU", "AUS", string.Empty, "Australia"));
            Add(new Region(14, "AW", "ABW", string.Empty, "Aruba"));
            Add(new Region(15, "AZ", "AZE", string.Empty, "Azerbaijan"));
            Add(new Region(16, "BA", "BIH", string.Empty, "Bosnia and Herzegovina"));
            Add(new Region(17, "BB", "BRB", string.Empty, "Barbados"));
            Add(new Region(18, "BD", "BGD", string.Empty, "Bangladesh"));
            Add(new Region(19, "BE", "BEL", string.Empty, "Belgium"));
            Add(new Region(20, "BF", "BFA", string.Empty, "Burkina Faso"));
            Add(new Region(21, "BG", "BGR", string.Empty, "Bulgaria"));
            Add(new Region(22, "BH", "BHR", string.Empty, "Bahrain"));
            Add(new Region(23, "BI", "BDI", string.Empty, "Burundi"));
            Add(new Region(24, "BJ", "BEN", string.Empty, "Benin"));
            Add(new Region(25, "BM", "BMU", string.Empty, "Bermuda"));
            Add(new Region(26, "BN", "BRN", string.Empty, "Brunei Darussalam"));
            Add(new Region(27, "BO", "BOL", string.Empty, "Bolivia"));
            Add(new Region(28, "BR", "BRA", string.Empty, "Brazil"));
            Add(new Region(29, "BS", "BHS", string.Empty, "Bahamas"));
            Add(new Region(30, "BT", "BTN", string.Empty, "Bhutan"));
            Add(new Region(31, "BV", "BVT", string.Empty, "Bouvet Island"));
            Add(new Region(32, "BW", "BWA", string.Empty, "Botswana"));
            Add(new Region(33, "BY", "BLR", string.Empty, "Belarus"));
            Add(new Region(34, "BZ", "BLZ", string.Empty, "Belize"));
            Add(new Region(35, "CA", "CAN", string.Empty, "Canada"));
            Add(new Region(36, "CC", "CCK", string.Empty, "Cocos  Islands"));
            Add(new Region(37, "CD", "COD", string.Empty, "Congo"));
            Add(new Region(38, "CF", "CAF", string.Empty, "Central African Republic"));
            Add(new Region(39, "CG", "COG", string.Empty, "Congo"));
            Add(new Region(40, "CH", "CHE", string.Empty, "Switzerland"));
            Add(new Region(41, "CI", "CIV", string.Empty, "Cote D'Ivoire"));
            Add(new Region(42, "CK", "COK", string.Empty, "Cook Islands"));
            Add(new Region(43, "CL", "CHL", string.Empty, "Chile"));
            Add(new Region(44, "CM", "CMR", string.Empty, "Cameroon"));
            Add(new Region(45, "CN", "CHN", string.Empty, "China"));
            Add(new Region(46, "CO", "COL", string.Empty, "Colombia"));
            Add(new Region(47, "CR", "CRI", string.Empty, "Costa Rica"));
            Add(new Region(48, "CU", "CUB", string.Empty, "Cuba"));
            Add(new Region(49, "CV", "CPV", string.Empty, "Cape Verde"));
            Add(new Region(50, "CX", "CXR", string.Empty, "Christmas Island"));
            Add(new Region(51, "CY", "CYP", string.Empty, "Cyprus"));
            Add(new Region(52, "CZ", "CZE", string.Empty, "Czech Republic"));
            Add(new Region(53, "DE", "DEU", string.Empty, "Germany"));
            Add(new Region(54, "DJ", "DJI", string.Empty, "Djibouti"));
            Add(new Region(55, "DK", "DNK", string.Empty, "Denmark"));
            Add(new Region(56, "DM", "DMA", string.Empty, "Dominica"));
            Add(new Region(57, "DO", "DOM", string.Empty, "Dominican Republic"));
            Add(new Region(58, "DZ", "DZA", string.Empty, "Algeria"));
            Add(new Region(59, "EC", "ECU", string.Empty, "Ecuador"));
            Add(new Region(60, "EE", "EST", string.Empty, "Estonia"));
            Add(new Region(61, "EG", "EGY", string.Empty, "Egypt"));
            Add(new Region(62, "EH", "ESH", string.Empty, "Western Sahara"));
            Add(new Region(63, "ER", "ERI", string.Empty, "Eritrea"));
            Add(new Region(64, "ES", "ESP", string.Empty, "Spain"));
            Add(new Region(65, "ET", "ETH", string.Empty, "Ethiopia"));
            Add(new Region(66, "FI", "FIN", string.Empty, "Finland"));
            Add(new Region(67, "FJ", "FJI", string.Empty, "Fiji"));
            Add(new Region(68, "FK", "FLK", string.Empty, "Falkland Islands"));
            Add(new Region(69, "FM", "FSM", string.Empty, "Micronesia"));
            Add(new Region(70, "FO", "FRO", string.Empty, "Faeroe Islands"));
            Add(new Region(71, "FR", "FRA", string.Empty, "France"));
            Add(new Region(72, "GA", "GAB", string.Empty, "Gabon"));
            Add(new Region(73, "GB", "GBR", string.Empty, "United Kingdom"));
            Add(new Region(74, "GD", "GRD", string.Empty, "Grenada"));
            Add(new Region(75, "GE", "GEO", string.Empty, "Georgia"));
            Add(new Region(76, "GF", "GUF", string.Empty, "French Guiana"));
            Add(new Region(77, "GH", "GHA", string.Empty, "Ghana"));
            Add(new Region(78, "GI", "GIB", string.Empty, "Gibraltar"));
            Add(new Region(79, "GL", "GRL", string.Empty, "Greenland"));
            Add(new Region(80, "GM", "GMB", string.Empty, "Gambia"));
            Add(new Region(81, "GN", "GIN", string.Empty, "Guinea"));
            Add(new Region(82, "GP", "GLP", string.Empty, "Guadaloupe"));
            Add(new Region(83, "GQ", "GNQ", string.Empty, "Equatorial Guinea"));
            Add(new Region(84, "GR", "GRC", string.Empty, "Greece"));
            Add(new Region(85, "GS", "SGS", string.Empty, "South Georgia and the South Sandwich Islands"));
            Add(new Region(86, "GT", "GTM", string.Empty, "Guatemala"));
            Add(new Region(87, "GU", "GUM", string.Empty, "Guam"));
            Add(new Region(88, "GW", "GNB", string.Empty, "Guinea-Bissau"));
            Add(new Region(89, "GY", "GUY", string.Empty, "Guyana"));
            Add(new Region(90, "HK", "HKG", string.Empty, "Hong Kong"));
            Add(new Region(91, "HM", "HMD", string.Empty, "Heard and McDonald Islands"));
            Add(new Region(92, "HN", "HND", string.Empty, "Honduras"));
            Add(new Region(93, "HR", "HRV", string.Empty, "Croatia"));
            Add(new Region(94, "HT", "HTI", string.Empty, "Haiti"));
            Add(new Region(95, "HU", "HUN", string.Empty, "Hungary"));
            Add(new Region(96, "ID", "IDN", string.Empty, "Indonesia"));
            Add(new Region(97, "IE", "IRL", string.Empty, "Ireland"));
            Add(new Region(98, "IL", "ISR", string.Empty, "Israel"));
            Add(new Region(99, "IN", "IND", string.Empty, "India"));
            Add(new Region(100, "IO", "IOT", string.Empty, "British Indian Ocean Territory"));
            Add(new Region(101, "IQ", "IRQ", string.Empty, "Iraq"));
            Add(new Region(102, "IR", "IRN", string.Empty, "Iran"));
            Add(new Region(103, "IS", "ISL", string.Empty, "Iceland"));
            Add(new Region(104, "IT", "ITA", string.Empty, "Italy"));
            Add(new Region(105, "JM", "JAM", string.Empty, "Jamaica"));
            Add(new Region(106, "JO", "JOR", string.Empty, "Jordan"));
            Add(new Region(107, "JP", "JPN", string.Empty, "Japan"));
            Add(new Region(108, "KE", "KEN", string.Empty, "Kenya"));
            Add(new Region(109, "KG", "KGZ", string.Empty, "Kyrgyz Republic"));
            Add(new Region(110, "KH", "KHM", string.Empty, "Cambodia"));
            Add(new Region(111, "KI", "KIR", string.Empty, "Kiribati"));
            Add(new Region(112, "KM", "COM", string.Empty, "Comoros"));
            Add(new Region(113, "KN", "KNA", string.Empty, "St. Kitts and Nevis"));
            Add(new Region(114, "KP", "PRK", string.Empty, "North Korea"));
            Add(new Region(115, "KR", "KOR", string.Empty, "South Korea"));
            Add(new Region(116, "KW", "KWT", string.Empty, "Kuwait"));
            Add(new Region(117, "KY", "CYM", string.Empty, "Cayman Islands"));
            Add(new Region(118, "KZ", "KAZ", string.Empty, "Kazakhstan"));
            Add(new Region(119, "LA", "LAO", string.Empty, "Lao People's Democratic Republic"));
            Add(new Region(120, "LB", "LBN", string.Empty, "Lebanon"));
            Add(new Region(121, "LC", "LCA", string.Empty, "St. Lucia"));
            Add(new Region(122, "LI", "LIE", string.Empty, "Liechtenstein"));
            Add(new Region(123, "LK", "LKA", string.Empty, "Sri Lanka"));
            Add(new Region(124, "LR", "LBR", string.Empty, "Liberia"));
            Add(new Region(125, "LS", "LSO", string.Empty, "Lesotho"));
            Add(new Region(126, "LT", "LTU", string.Empty, "Lithuania"));
            Add(new Region(127, "LU", "LUX", string.Empty, "Luxembourg"));
            Add(new Region(128, "LV", "LVA", string.Empty, "Latvia"));
            Add(new Region(129, "LY", "LBY", string.Empty, "Libyan Arab Jamahiriya"));
            Add(new Region(130, "MA", "MAR", string.Empty, "Morocco"));
            Add(new Region(131, "MC", "MCO", string.Empty, "Monaco"));
            Add(new Region(132, "MD", "MDA", string.Empty, "Moldova"));
            Add(new Region(133, "ME", "MNE", string.Empty, "Montenegro"));
            Add(new Region(134, "MG", "MDG", string.Empty, "Madagascar"));
            Add(new Region(135, "MH", "MHL", string.Empty, "Marshall Islands"));
            Add(new Region(136, "MK", "MKD", string.Empty, "Macedonia"));
            Add(new Region(137, "ML", "MLI", string.Empty, "Mali"));
            Add(new Region(138, "MM", "MMR", string.Empty, "Myanmar"));
            Add(new Region(139, "MN", "MNG", string.Empty, "Mongolia"));
            Add(new Region(140, "MO", "MAC", string.Empty, "Macao"));
            Add(new Region(141, "MP", "MNP", string.Empty, "Northern Mariana Islands"));
            Add(new Region(142, "MQ", "MTQ", string.Empty, "Martinique"));
            Add(new Region(143, "MR", "MRT", string.Empty, "Mauritania"));
            Add(new Region(144, "MS", "MSR", string.Empty, "Montserrat"));
            Add(new Region(145, "MT", "MLT", string.Empty, "Malta"));
            Add(new Region(146, "MU", "MUS", string.Empty, "Mauritius"));
            Add(new Region(147, "MV", "MDV", string.Empty, "Maldives"));
            Add(new Region(148, "MW", "MWI", string.Empty, "Malawi"));
            Add(new Region(149, "MX", "MEX", string.Empty, "Mexico"));
            Add(new Region(150, "MY", "MYS", string.Empty, "Malaysia"));
            Add(new Region(151, "MZ", "MOZ", string.Empty, "Mozambique"));
            Add(new Region(152, "NA", "NAM", string.Empty, "Namibia"));
            Add(new Region(153, "NC", "NCL", string.Empty, "New Caledonia"));
            Add(new Region(154, "NE", "NER", string.Empty, "Niger"));
            Add(new Region(155, "NF", "NFK", string.Empty, "Norfolk Island"));
            Add(new Region(156, "NG", "NGA", string.Empty, "Nigeria"));
            Add(new Region(157, "NI", "NIC", string.Empty, "Nicaragua"));
            Add(new Region(158, "NL", "NLD", string.Empty, "Netherlands"));
            Add(new Region(159, "NO", "NOR", string.Empty, "Norway"));
            Add(new Region(160, "NP", "NPL", string.Empty, "Nepal"));
            Add(new Region(161, "NR", "NRU", string.Empty, "Nauru"));
            Add(new Region(162, "NU", "NIU", string.Empty, "Niue"));
            Add(new Region(163, "NZ", "NZL", string.Empty, "New Zealand"));
            Add(new Region(164, "OM", "OMN", string.Empty, "Oman"));
            Add(new Region(165, "PA", "PAN", string.Empty, "Panama"));
            Add(new Region(166, "PE", "PER", string.Empty, "Peru"));
            Add(new Region(167, "PF", "PYF", string.Empty, "French Polynesia"));
            Add(new Region(168, "PG", "PNG", string.Empty, "Papua New Guinea"));
            Add(new Region(169, "PH", "PHL", string.Empty, "Philippines"));
            Add(new Region(170, "PK", "PAK", string.Empty, "Pakistan"));
            Add(new Region(171, "PL", "POL", string.Empty, "Poland"));
            Add(new Region(172, "PM", "SPM", string.Empty, "St. Pierre and Miquelon"));
            Add(new Region(173, "PN", "PCN", string.Empty, "Pitcairn Island"));
            Add(new Region(174, "PR", "PRI", string.Empty, "Puerto Rico"));
            Add(new Region(175, "PS", "PSE", string.Empty, "Palestinian Territory"));
            Add(new Region(176, "PT", "PRT", string.Empty, "Portugal"));
            Add(new Region(177, "PW", "PLW", string.Empty, "Palau"));
            Add(new Region(178, "PY", "PRY", string.Empty, "Paraguay"));
            Add(new Region(179, "QA", "QAT", string.Empty, "Qatar"));
            Add(new Region(180, "RE", "REU", string.Empty, "Reunion"));
            Add(new Region(181, "RO", "ROU", string.Empty, "Romania"));
            Add(new Region(182, "RS", "SRB", string.Empty, "Serbia"));
            Add(new Region(183, "RU", "RUS", string.Empty, "Russia"));
            Add(new Region(184, "RW", "RWA", string.Empty, "Rwanda"));
            Add(new Region(185, "SA", "SAU", string.Empty, "Saudi Arabia"));
            Add(new Region(186, "SB", "SLB", string.Empty, "Solomon Islands"));
            Add(new Region(187, "SC", "SYC", string.Empty, "Seychelles"));
            Add(new Region(188, "SD", "SDN", string.Empty, "Sudan"));
            Add(new Region(189, "SE", "SWE", string.Empty, "Sweden"));
            Add(new Region(190, "SG", "SGP", string.Empty, "Singapore"));
            Add(new Region(191, "SH", "SHN", string.Empty, "St. Helena"));
            Add(new Region(192, "SI", "SVN", string.Empty, "Slovenia"));
            Add(new Region(193, "SJ", "SJM", string.Empty, "Svalbard & Jan Mayen Islands"));
            Add(new Region(194, "SK", "SVK", string.Empty, "Slovakia"));
            Add(new Region(195, "SL", "SLE", string.Empty, "Sierra Leone"));
            Add(new Region(196, "SM", "SMR", string.Empty, "San Marino"));
            Add(new Region(197, "SN", "SEN", string.Empty, "Senegal"));
            Add(new Region(198, "SO", "SOM", string.Empty, "Somalia"));
            Add(new Region(199, "SR", "SUR", string.Empty, "Suriname"));
            Add(new Region(200, "SS", "SSD", string.Empty, "South Sudan"));
            Add(new Region(201, "ST", "STP", string.Empty, "Sao Tome and Principe"));
            Add(new Region(202, "SV", "SLV", string.Empty, "El Salvador"));
            Add(new Region(203, "SY", "SYR", string.Empty, "Syrian Arab Republic"));
            Add(new Region(204, "SZ", "SWZ", string.Empty, "Swaziland"));
            Add(new Region(205, "TC", "TCA", string.Empty, "Turks and Caicos Islands"));
            Add(new Region(206, "TD", "TCD", string.Empty, "Chad"));
            Add(new Region(207, "TF", "ATF", string.Empty, "French Southern Territories"));
            Add(new Region(208, "TG", "TGO", string.Empty, "Togo"));
            Add(new Region(209, "TH", "THA", string.Empty, "Thailand"));
            Add(new Region(210, "TJ", "TJK", string.Empty, "Tajikistan"));
            Add(new Region(211, "TK", "TKL", string.Empty, "Tokelau"));
            Add(new Region(212, "TL", "TLS", string.Empty, "Timor-Leste"));
            Add(new Region(213, "TM", "TKM", string.Empty, "Turkmenistan"));
            Add(new Region(214, "TN", "TUN", string.Empty, "Tunisia"));
            Add(new Region(215, "TO", "TON", string.Empty, "Tonga"));
            Add(new Region(216, "TR", "TUR", string.Empty, "Turkey"));
            Add(new Region(217, "TT", "TTO", string.Empty, "Trinidad and Tobago"));
            Add(new Region(218, "TV", "TUV", string.Empty, "Tuvalu"));
            Add(new Region(219, "TW", "TWN", string.Empty, "Taiwan"));
            Add(new Region(220, "TZ", "TZA", string.Empty, "Tanzania"));
            Add(new Region(221, "UA", "UKR", string.Empty, "Ukraine"));
            Add(new Region(222, "UG", "UGA", string.Empty, "Uganda"));
            Add(new Region(223, "UM", "UMI", string.Empty, "United States Minor Outlying Islands"));
            Add(new Region(224, "US", "USA", string.Empty, "United States of America"));
            Add(new Region(225, "UY", "URY", string.Empty, "Uruguay"));
            Add(new Region(226, "UZ", "UZB", string.Empty, "Uzbekistan"));
            Add(new Region(227, "VA", "VAT", string.Empty, "Holy See"));
            Add(new Region(228, "VC", "VCT", string.Empty, "St. Vincent and the Grenadines"));
            Add(new Region(229, "VE", "VEN", string.Empty, "Venezuela"));
            Add(new Region(230, "VG", "VGB", string.Empty, "British Virgin Islands"));
            Add(new Region(231, "VI", "VIR", string.Empty, "US Virgin Islands"));
            Add(new Region(232, "VN", "VNM", string.Empty, "Vietnam"));
            Add(new Region(233, "VU", "VUT", string.Empty, "Vanuatu"));
            Add(new Region(234, "WF", "WLF", string.Empty, "Wallis and Futuna Islands"));
            Add(new Region(235, "WS", "WSM", string.Empty, "Samoa"));
            Add(new Region(236, "YE", "YEM", string.Empty, "Yemen"));
            Add(new Region(237, "YT", "MYT", string.Empty, "Mayotte"));
            Add(new Region(238, "ZA", "ZAF", string.Empty, "South Africa"));
            Add(new Region(239, "ZM", "ZMB", string.Empty, "Zambia"));
            Add(new Region(240, "ZW", "ZWE", string.Empty, "Zimbabwe"));
        }

        public Region? RegionFromCode(string regionCode)
        {
            return this.SingleOrDefault(x => x.Abbreviation == regionCode);
        }

        public Region? RegionFromName(string? regionName)
        {
            return this.SingleOrDefault(x => x.EnglishName == regionName);
        }

        public Region? RegionFrom3Code(string region3Code)
        {
            return this.SingleOrDefault(x => x.ThreeAbbreviation == region3Code);
        }
    }
}
