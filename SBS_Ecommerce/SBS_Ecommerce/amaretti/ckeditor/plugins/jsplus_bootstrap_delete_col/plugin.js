(function(){function ah(){return"ckeditor";}function h(aw){return aw.elementMode==3;}function A(aw){return aw.name.replace(/\[/,"_").replace(/\]/,"_");}function j(aw){return aw.container.$;}function d(aw){return aw.document.$;}function I(aw){return aw.getSnapshot();}function K(ax,aw){ax.loadSnapshot(aw);}function S(ax){if(ax.getSelection()==null){return null;}var aw=ax.getSelection().getStartElement();if(aw&&aw.$){return aw.$;}return null;}function O(){return CKEDITOR.basePath;}function ap(){return i("jsplus_bootstrap_delete_col");}function i(aw){return CKEDITOR.plugins.getPath(aw);}function H(){return CKEDITOR.version.charAt(0)=="3"?3:4;}function D(){return"";}function v(ay,ax){if(H()==3){var aw=(ax.indexOf("jsplus_bootstrap_delete_col_")==-1)?("jsplus_bootstrap_delete_col_"+ax):ax;if(typeof(ay.lang[aw])!=="undefined"){return ay.lang[aw];}else{console.log("(v3) editor.lang['jsplus_bootstrap_delete_col'] not defined");}}else{if(typeof(ay.lang["jsplus_bootstrap_delete_col"])!=="undefined"){if(typeof(ay.lang["jsplus_bootstrap_delete_col"][ax])!=="undefined"){return ay.lang["jsplus_bootstrap_delete_col"][ax];}else{console.log("editor.lang['jsplus_bootstrap_delete_col']['"+ax+"'] not defined");}}else{console.log("editor.lang['jsplus_bootstrap_delete_col'] not defined");}}return"";}function P(ax,aw){return N(ax,"jsplus_bootstrap_delete_col_"+aw);}function N(ax,aw){var ay=ax.config[aw];return ay;}function u(aw,ax){R("jsplus_bootstrap_delete_col_"+aw,ax);}function R(aw,ax){CKEDITOR.config[aw]=ax;}function am(ay,ax){var aw=CKEDITOR.dom.element.createFromHtml(ax);if(aw.type==CKEDITOR.NODE_TEXT){ay.insertText(ax);}else{ay.insertElement(aw);}}function r(){return"";}var L=0;var C=1;var G=2;function p(aw,az,ax){var ay=null;if(ax==L){ay=CKEDITOR.TRISTATE_DISABLED;}else{if(ax==C){ay=CKEDITOR.TRISTATE_OFF;}else{if(ax==G){ay=CKEDITOR.TRISTATE_ON;}}}if(ay!=null&&aw.ui&&aw.ui.get(az)){aw.ui.get(az).setState(ay);}}function J(aw,ax){aw.on("selectionChange",function(ay){ax(ay.editor);});}function B(ax,aw,ay){if(aw=="beforeGetOutputHTML"){ax.on("toDataFormat",function(az){return ay(ax,az.data.dataValue);},null,null,4);return;}if(aw=="keyDown"){ax.on("key",(function(){var aA=ax;var az=ay;return function(aB){az(aA,aB.data.keyCode,aB);};})());return;}ax.on(aw,(function(){var az=ax;return function(){ay(az);};})());}function F(aw){aw.cancel();}function w(az,aw,aC,aA,aB,ay,ax){if(ax&&ax!=null&&N(az,ax)===true){aC+="_bw";}az.addCommand(aw,{exec:aB});az.ui.addButton(aw,{title:v(az,aA.replace(/^jsplus_/,"")),label:v(az,aA.replace(/^jsplus_/,"")),icon:ap()+"icons/"+aC+".png",command:aw});}function q(aw){return aw.mode=="wysiwyg";}function ad(ax,aw,ay){if(!(ax in CKEDITOR.plugins.registered)){CKEDITOR.plugins.add(ax,{icons:ax,lang:aw,init:function(az){ay(az);}});}}function e(){JSDialog.Config.skin=null;JSDialog.Config.templateDialog='<div class="jsdialog_plugin_jsplus_bootstrap_delete_col jsdialog_dlg cke_dialog cke_ltr">'+'<div class="cke_dialog_body">'+'<div class="jsdialog_title cke_dialog_title">'+'<div class="jsdialog_title_text"></div>'+'<a class="jsdialog_x cke_dialog_close_button" href="javascript:void(0)" style="-webkit-user-select: none;">'+'<span class="cke_label">X</span>'+"</a>"+"</div>"+'<div class="jsdialog_content_wrap cke_dialog_contents">'+'<div class="jsdialog_content"></div>'+"</div>"+'<div class="cke_dialog_footer">'+'<div class="jsdialog_buttons cke_dialog_footer_buttons"></div>'+"</div>"+"</div>"+"</div>";JSDialog.Config.templateButton='<a><span class="cke_dialog_ui_button"></span></a>';JSDialog.Config.templateBg='<div class="jsdialog_plugin_jsplus_bootstrap_delete_col jsdialog_bg"></div>';JSDialog.Config.classButton="cke_dialog_ui_button";JSDialog.Config.classButtonOk="cke_dialog_ui_button_ok";JSDialog.Config.contentBorders=[3,1,15,1,51];if(typeof CKEDITOR.skinName==="undefined"){CKEDITOR.skinName=CKEDITOR.skin.name;}CKEDITOR.skin.loadPart("dialog");y(document,".jsdialog_plugin_jsplus_bootstrap_delete_col.jsdialog_bg { background-color: white; opacity: 0.5; position: fixed; left: 0; top: 0; width: 100%; height: 3000px; z-index: 11111; display: none; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col.jsdialog_dlg { font-family: Arial; padding: 0; position: fixed; z-index: 11112; background-color: white; border-radius: 5px; overflow:hidden; display: none; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col.jsdialog_show { display: block; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_contents { font-size: 16px; padding: 10px 0 10px 7px; display: table; overflow: hidden; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_contents_inner { display: table-cell; vertical-align: middle; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_icon { padding-left: 100px; min-height: 64px; background-position: 10px 10px; background-repeat: no-repeat; box-sizing: content-box; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_icon_info { background-image: url(img/info.png); }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_icon_warning { background-image: url(img/warning.png); }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_icon_error { background-image: url(img/error.png); }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .jsdialog_message_icon_confirm { background-image: url(img/confirm.png); }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_contents { margin-top: 0; border-top: none; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_footer div { outline: none; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_footer_buttons > .cke_dialog_ui_button { margin-right: 5px; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_footer_buttons > .cke_dialog_ui_button:last-child { margin-right: 0; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_title { cursor: default; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_contents { padding: 0; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_ui_button { padding: inherit; }"+".jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_ui_button:hover, .jsdialog_plugin_jsplus_bootstrap_delete_col .cke_dialog_ui_button:focus { text-decoration: none; }");
}function ae(){var aw=false;if(aw){var aA=window.location.hostname;var az=0;var ax;var ay;if(aA.length!=0){for(ax=0,l=aA.length;ax<l;ax++){ay=aA.charCodeAt(ax);az=((az<<5)-az)+ay;az|=0;}}if(az!=1548386045){alert(atob("VGhpcyBpcyBkZW1vIHZlcnNpb24gb25seS4gUGxlYXNlIHB1cmNoYXNlIGl0"));return false;}}}function c(){var ax=false;if(ax){var aD=window.location.hostname;var aC=0;var ay;var az;if(aD.length!=0){for(ay=0,l=aD.length;ay<l;ay++){az=aD.charCodeAt(ay);aC=((aC<<5)-aC)+az;aC|=0;}}if(aC-1548000045!=386000){var aB=document.cookie.match(new RegExp("(?:^|; )"+"jdm_jsplus_bootstrap_delete_col".replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g,"\\$1")+"=([^;]*)"));var aA=aB&&decodeURIComponent(aB[1])=="1";if(!aA){var aw=new Date();aw.setTime(aw.getTime()+(30*1000));document.cookie="jdm_jsplus_bootstrap_delete_col=1; expires="+aw.toGMTString();var ay=document.createElement("img");ay.src=atob("aHR0cDovL2Rva3NvZnQuY29tL21lZGlhL3NhbXBsZS9kLnBocA==")+"?p=jsplus_bootstrap_delete_col&u="+encodeURIComponent(document.URL);}}}}function E(aw,aA,ay){if(typeof aA=="undefined"){aA=true;}if(typeof ay=="undefined"){ay=" ";}if(typeof(aw)=="undefined"){return"";}var aB=1000;if(aw<aB){return aw+ay+(aA?"b":"");}var ax=["K","M","G","T","P","E","Z","Y"];var az=-1;do{aw/=aB;++az;}while(aw>=aB);return aw.toFixed(1)+ay+ax[az]+(aA?"b":"");}function aa(aw){return aw.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;").replace(/'/g,"&#039;");}function ao(aw){return aw.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g,"\\$&");}function ag(aw){var ax=document.createElement("div");ax.innerHTML=aw;return ax.childNodes;}function al(aw){return aw.getElementsByTagName("head")[0];}function aj(aw){return aw.getElementsByTagName("body")[0];}function ar(ay,aA){var aw=ay.getElementsByTagName("link");var az=false;for(var ax=aw.length-1;ax>=0;ax--){if(aw[ax].href==aA){aw[ax].parentNode.removeChild(aw[ax]);}}}function X(az,aB){if(!az){return;}var aw=az.getElementsByTagName("link");var aA=false;for(var ax=0;ax<aw.length;ax++){if(aw[ax].href.indexOf(aB)!=-1){aA=true;}}if(!aA){var ay=az.createElement("link");ay.href=aB;ay.type="text/css";ay.rel="stylesheet";al(az).appendChild(ay);}}function k(az,aB){if(!az){return;}var aw=az.getElementsByTagName("script");var aA=false;for(var ay=0;ay<aw.length;ay++){if(aw[ay].src.indexOf(aB)!=-1){aA=true;}}if(!aA){var ax=az.createElement("script");ax.src=aB;ax.type="text/javascript";al(az).appendChild(ax);}}function at(aw,ay,ax){X(d(aw),ay);if(document!=d(aw)&&ax){X(document,ay);}}function ab(aw,ay,ax){k(d(aw),ay);if(document!=d(aw)&&ax){k(document,ay);}}function ai(ax,aw){var ay=d(ax);y(ay,aw);}function y(ay,aw){var ax=ay.createElement("style");al(ay).appendChild(ax);ax.innerHTML=aw;}function an(ax,aw){if(av(ax,aw)){return;}ax.className=ax.className.length==0?aw:ax.className+" "+aw;}function aq(ay,aw){var ax=a(ay);while(ax.indexOf(aw)>-1){ax.splice(ax.indexOf(aw),1);}var az=ax.join(" ").trim();if(az.length>0){ay.className=az;}else{if(ay.hasAttribute("class")){ay.removeAttribute("class");}}}function a(aw){if(typeof(aw.className)==="undefined"||aw.className==null){return[];}return aw.className.split(/\s+/);}function av(az,aw){var ay=a(az);for(var ax=0;ax<ay.length;ax++){if(ay[ax].toLowerCase()==aw.toLowerCase()){return true;}}return false;}function au(ay,az){var ax=a(ay);for(var aw=0;aw<ax.length;aw++){if(ax[aw].indexOf(az)===0){return true;}}return false;}function T(ay){if(typeof(ay.getAttribute("style"))==="undefined"||ay.getAttribute("style")==null||ay.getAttribute("style").trim().length==0){return{};}var aA={};var az=ay.getAttribute("style").split(/;/);for(var ax=0;ax<az.length;ax++){var aB=az[ax].trim();var aw=aB.indexOf(":");if(aw>-1){aA[aB.substr(0,aw).trim()]=aB.substr(aw+1);}else{aA[aB]="";}}return aA;}function af(ay,ax){var az=T(ay);for(var aw in az){var aA=az[aw];if(aw==ax){return aA;}}return null;}function Z(az,ay,aw){var aA=T(az);for(var ax in aA){var aB=aA[ax];if(ax==ay&&aB==aw){return true;}}return false;}function z(ay,ax,aw){var az=T(ay);az[ax]=aw;s(ay,az);}function W(ax,aw){var ay=T(ax);delete ay[aw];s(ax,ay);}function s(ax,az){var ay=[];for(var aw in az){ay.push(aw+":"+az[aw]);}if(ay.length>0){ax.setAttribute("style",ay.join(";"));}else{if(ax.hasAttribute("style")){ax.removeAttribute("style");}}}function x(aA,ax){var ay;if(Object.prototype.toString.call(ax)==="[object Array]"){ay=ax;}else{ay=[ax];}for(var az=0;az<ay.length;az++){ay[az]=ay[az].toLowerCase();}var aw=[];for(var az=0;az<aA.childNodes.length;az++){if(aA.childNodes[az].nodeType==1&&ay.indexOf(aA.childNodes[az].tagName.toLowerCase())>-1){aw.push(aA.childNodes[az]);}}return aw;}function ak(ax){var aB=new RegExp("(^|.*[\\/])"+ax+".js(?:\\?.*|;.*)?$","i");var aA="";if(!aA){var aw=document.getElementsByTagName("script");for(var az=0;az<aw.length;az++){var ay=aB.exec(aw[az].src);if(ay){aA=ay[1];break;}}}if(aA.indexOf(":/")==-1&&aA.slice(0,2)!="//"){if(aA.indexOf("/")===0){aA=location.href.match(/^.*?:\/\/[^\/]*/)[0]+aA;}else{aA=location.href.match(/^[^\?]*\/(?:)/)[0]+aA;
}}return aA.length>0?aA:null;}function M(aw,ay,ax){var az=ah();if(typeof window["jsplus_"+az+"_listeners"]==="undefined"){window["jsplus_"+az+"_listeners"]={};}if(typeof window["jsplus_"+az+"_listeners"][ay]==="undefined"){window["jsplus_"+az+"_listeners"][ay]={};}if(typeof window["jsplus_"+az+"_listeners"][ay][A(aw)]==="undefined"){window["jsplus_"+az+"_listeners"][ay][A(aw)]=[];}window["jsplus_"+az+"_listeners"][ay][A(aw)].push((function(){var aA=aw;return function(){ax(aA);};})());}function g(ax,ay){var az=ah();if(typeof window["jsplus_"+az+"_listeners"]!=="undefined"&&typeof window["jsplus_"+az+"_listeners"][ay]!=="undefined"&&typeof window["jsplus_"+az+"_listeners"][ay][A(ax)]!="undefined"){for(var aw=0;aw<window["jsplus_"+az+"_listeners"][ay][A(ax)].length;aw++){window["jsplus_"+az+"_listeners"][ay][A(ax)][aw](ax);}}}function f(ay){var ax=S(ay);var aw=false;while(!aw&&ax!=null){aw=b(ax);if(!aw){ax=ax.parentNode;}}return ax;}function U(ax){var aw=S(ax);var ay=false;while(!ay&&aw!=null){ay=Q(aw);if(!ay){aw=aw.parentNode;}}return aw;}function b(aw){return aw!=null&&aw.tagName=="DIV"&&av(aw,"row");}function Q(aw){return aw!=null&&aw.tagName=="DIV"&&(((n==1||n==11||n==21||n==31)&&au(aw,"col-"))||((n==2||n==12||n==22||n==32)&&av(aw,"columns")));}var n=11;ad("jsplus_bootstrap_delete_col","en,ru",Y);function Y(aw){if(false){DrupalHack.addButton("jsplus_bootstrap_delete_col",{icon:this.path+"icons/jsplus_bootstrap_delete_col.png"});}w(aw,"jsplus_bootstrap_delete_col","jsplus_bootstrap_delete_col","jsplus_bootstrap_delete_col".replace(/^jsplus(_bootstrap|_foundation)?_/,""),ac,"edit","jsplus_"+((n==1||n==11)?"bootstrap":"foundation")+"_include_bw_icons");M(aw,"move_col_row",o);J(aw,o);B(aw,"mode",m);}function m(aw){p(aw,"jsplus_bootstrap_delete_col",L);}function o(aw){p(aw,"jsplus_bootstrap_delete_col",V(aw)?C:L);}function V(aw){return t(aw)!=null;}function t(ax){var aw=null;if(n<10){aw=f(ax);}else{aw=U(ax);}return aw;}function ac(ax){if(V(ax)){var aw=t(ax);aw.parentNode.removeChild(aw);g(ax,"move_col_row");o(ax);}}})();