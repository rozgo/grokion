
function Update () {

var offset = Time.time * 0.02;
    renderer.material.mainTextureOffset = Vector2 (offset,offset);
    
     var scaleX = 6;
    var scaleY = 6;
    renderer.material.mainTextureScale = Vector2 (scaleX,scaleY);
    
 

}