# Appleseed Search Web (Legacy)
Appleseed Search Web User legacy components : WCF SVC Web.API that talks to Lucene, KnockoutJS interface, ASP.net User Controls that talk to Lucene

These are deprecated interfaces that talk to Lucene indexes created by Appleseed.Search.Engine. Since we now support SolR , Elastic, and soon other indexes that have their own API layers, 
we decided not to support a Lucene.NET API or interfaces that talk to it. 

We may at some point create a facade API to replace this that can equally talk to Lucene, Elastic, SolR, etc. For now this code will have to stay in code purgatory. 

##Features##

* Single Page Applications 
* AngularJS Based (AngularJS 1.5 w/ Components /AngularJS 2 Coming)
* Quickstart search for Elastic Search
* Quickstart search for SolR 

#Where it began#

When trying to bring up a search engine interface for one of our clients at [Anant](https://www.anant.us/), 
I looked for different ways to get a simple interface to look at results that weren't in JSON from Elastic or SolR.
I found some good libraries, but the problem was the same. All the things needed to get started in open source search 
were on the internet, but it takes forever to do anything useful. 

This project is a collection of subprojects built over time at [Anant](https://www.anant.us/) by different [team members](https://www.anant.us/Company.aspx). 
