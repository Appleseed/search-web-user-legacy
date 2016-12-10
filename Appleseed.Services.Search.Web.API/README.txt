This is where we are centralizing the API endpoints for Search.Web.API for BSR. 

1. It will talk to the Solr server and expose the basic endpoints that solr exposes but as a proxy. 
2. It will be refactored into SearchService.svc as SolrSearchService 
3. It will be all refactored to expose as both WCF and WebAPI



This below code needs to add inside solrconfig.xml


  
<!-- Suggester

       http://wiki.apache.org/solr/Suggester
    -->  
  
  
  <searchComponent class="solr.SpellCheckComponent" name="suggest">
    <lst name="spellchecker">
      <str name="name">suggest</str>
      <str name="classname">org.apache.solr.spelling.suggest.Suggester</str>
      <str name="lookupImpl">org.apache.solr.spelling.suggest.tst.TSTLookupFactory</str>
      <!-- Alternatives to lookupImpl: 
           org.apache.solr.spelling.suggest.fst.FSTLookupFactory   [finite state automaton]
           org.apache.solr.spelling.suggest.fst.WFSTLookupFactory [weighted finite state automaton]
           org.apache.solr.spelling.suggest.jaspell.JaspellLookupFactory [default, jaspell-based]
           org.apache.solr.spelling.suggest.tst.TSTLookupFactory   [ternary trees]
      -->
      <str name="field">name</str>  <!-- the indexed field to derive suggestions from -->
      <float name="threshold">0.005</float>
      <str name="buildOnCommit">true</str>
<!--
      <str name="sourceLocation">american-english</str>
-->
    </lst>
  </searchComponent>
  <requestHandler class="org.apache.solr.handler.component.SearchHandler" name="/suggest">
    <lst name="defaults">
      <str name="spellcheck">true</str>
      <str name="spellcheck.dictionary">suggest</str>
      <str name="spellcheck.onlyMorePopular">true</str>
      <str name="spellcheck.count">5</str>
      <str name="spellcheck.collate">true</str>
    </lst>
    <arr name="components">
      <str>suggest</str>
    </arr>
  </requestHandler>
  
  
  