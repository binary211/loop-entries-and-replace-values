//add using
//add reference to delivery and management APIs
//initilize clients

string[] langs = {"en-IN", "en-US", "en-GB", "zh", "nl", "en-CA","fr","fr-NL","de","ko","pt","es","es-US", "es-MX","sv" };
                        
            foreach (var lang in langs){
                var ncAuthor = client.Entries.Get("a208bea0-ebd4-4164-8ef9-12c7300a5d2b", lang, 1);
                var dominoAuthor = client.Entries.Get("85033474-6df0-4578-a601-0a01a069fe19", lang, 1);
                if(lang == "en-US"){dominoAuthor = client.Entries.Get("67c3d9a5-d20c-484c-b38e-3c0f8594ab75");}
                if(ncAuthor != null){
                    var query2 = new Zengenti.Search.Query(
                        Op.EqualTo("sys.contentTypeId", "blog"),
                        Op.EqualTo("sys.versionStatus", "published"),
                        Op.EqualTo("sys.language", lang)
                    );
                    query2.PageIndex = 0;
                    query2.PageSize = 999;
                    var articlesList = client.Entries.Search(query2,1).Items;
                    
                    var articles = articlesList.Where(r=>r.Language == lang && r.Get<Entry>("authorPicker").Id == ncAuthor.Id).ToList();
                    foreach (Entry article in articles.Where(a=>a.Version.Published != null && Convert.ToDecimal(a.Version.VersionNo) >= 1 )){
                        var ncArticleId = article.Id;
                        var manEntry = mng_client.Entries.Get(ncArticleId, lang);
                        var currentVersion = manEntry.Version.VersionNo;
                        manEntry.Set("authorPicker", dominoAuthor);
                        try{
                            manEntry.Save();
                        }
                        catch(Exception ex){
                            System.Console.WriteLine("Error with: https://cms-domino-printing.cloud.contensis.com/Default.aspx#/projects/website/entries/" + manEntry.Id + "/" + lang );
                            System.Console.WriteLine(ex.Message);
                        }
                        
                        if(manEntry.Version.VersionNo != currentVersion){
                         //   manEntry.Workflow.Publish();
                        }

                    }
                }
