# A Pokemon GO Clone Game

## overview

![image](http://img.blog.csdn.net/20170228140250518?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQvbXNlb2w=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/Center)

## Update plugins

### if GOMap plugin update, do this following,

1. filled mapzen_api_key： mapzen-Zj6sQpm

2. FourSquare POI 

```

string url = baseUrl + "&ll=" + currentLocation.latitude + "," + currentLocation.longitude + "&query=" + queryPOI + "&intent=" + "checkin" + "&client_id=" + categoryID + "&client_secret=" + oauth_token;

```         

3. add using GoShared; to use class Coordinates.

keep an eye on [my blog](http://blog.csdn.net/mseol/article/details/53463981). I will tell many technologies and details of this game.
