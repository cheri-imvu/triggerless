// Write JavaScript here and press Ctrl+Enter to execute

const TitleBar = (props) => {
	return (
  	<div className="title_bar">
    	<div className="years_block">{props.imvu_level}</div>
      <div className="years_text">YEARS<br/>AT IMVU</div>
      <div className="avatar_title">{props.avname}</div>
    </div>
  );
};

const Body = (props) => {
  var wpclass = 'body show-paper wallpaper' + props.content.wallpaper_id;
	return (
  	<div className={wpclass}>
    	<div>
      	<img className="avatar_image" src={props.content.avpic_url} />
        <UserInfo content={props.content} />
      </div>
      <BadgeContainer badges={props.content.badge_layout} count={props.content.badge_count} avname={props.content.avname} />
    </div>
  );
}

const UserInfo = (props) => {
  let age = props.content.age === 'NA' || props.content.age === 'N/A' ? null : props.content.age;
  let gender = props.content.gender === 'Female' || props.content.gender === 'Male' ? props.content.gender : null;
  let location = props.content.location.replace(/<\/span>/g, '').replace(/<span class="SL_block">/g, '').replace(/<span class='notranslate'>/g, '');
  
	return (
  	<div className="user_info">
    	<div className="info_avname" style={{fontWeight: 700}}>{props.content.avname}</div>
      <InfoSprites content={props.content} />
      <div style={{fontSize: 12, marginTop: 10}}>
      	{gender &&
        	<span>{gender}</span>
        }
        {gender && age &&
        	<span> / </span>
        }
        {age &&
        	<span>Age: {age}</span>
        }
      </div>
      <div style={{fontSize: 12}}>
        {location}
      </div>
      {props.content.show_marriage &&
      	<div style={{fontSize: 12}}>Married to <span className="link">{props.content.married_to_partner_avname}</span></div>
      }
      <div style={{fontSize: 12, marginTop: 10}}>
      	{props.content.tagline}
      </div>
      <div style={{fontSize: 14, marginTop: 10}}>
      	Availability: 
        {props.content.online &&
          	<span className="online-img"></span>
        }
        {!props.content.online &&
        	<span>&nbsp;Offline</span>
        }
      </div>
      {props.content.is_creator &&
      	<div style={{marginTop: 10}}>
        	<span className="shop-icon"/>
          <span className="link shop-link">Visit my shop</span>
          <span className="shop-arrow" />
        </div>
      }
    </div>
  );
};

const InfoSprites = (props) => {
	let badgeClassNames = 'badgecount-sprite level-0';
	return (
  	<div className="info_sprites">
    	{props.content.show_ap &&
      	<span className="ap-sprite" />
      }
    	{props.content.show_ageverify &&
      	<span className="av-sprite" />
      }
    	{props.content.show_vip &&
      	<span className="vip-sprite" />
      }
      {props.content.show_badgecount &&
      	<span className={badgeClassNames}>{props.content.badge_count}</span>
      }
      {props.content.show_marriage &&
      	<span className="marriage-sprite" />
      }
    </div>
  );
};

const BadgeContainer = (props) => {
  let badgeKeys = [];
  for (var propName in props.badges) if (props.badges.hasOwnProperty(propName)) badgeKeys.push(propName);
	return (
  	<div>
    	<div className="badge-text">{props.avname}'s badges ({props.count})</div>
      <div className="badge-panel">
        {badgeKeys.map((propName, i) => (
        	<Badge key={i} badge={props.badges[propName]}>
          </Badge>
        ))}
      </div>
    </div>
  );
};

const Badge = (props) => {
	return (
  	<div 
      style={{
        position: 'absolute',
        width: props.badge.image_width,
        height: props.badge.image_height,
        left: props.badge.xloc,
        top: props.badge.yloc,
        backgroundImage: 'url(' + props.badge.image_url + ')',
        cursor: 'pointer'
      }}
      title = {props.badge.name}
    />
  );
}

class App extends React.Component {
	state = {
  "viewer_cid": 83079851,
  "cid": 34740890,
  "is_buddy": false,
  "is_friend": false,
  "is_qa": false,
  "avname": "Sexii",
  "url": "http://avatars.imvu.com/Sexii",
  "avpic_url": "http://userimages-akm.imvu.com/catalog/includes/modules/phpbb2/images/avatars/34740890_15090701195946009406d31.jpg",
  "registered": "2009-02-22",
  "last_login": "03/27/18",
  "interests": {
    "full_text_string": {
      "tag": "",
      "raw_tag": "I was born underwater...with 3 dollars and six dimes...yea u may laugh..cuz ya did not do ur math ;)"
    }
  },
  "dating": {
    "relationship_status": "Married",
    "orientation": "Prefer Not To Say",
    "looking_for": "Prefer Not To Say"
  },
  "gender": "Female",
  "age": "NA",
  "tagline": "~Sighs~",
  "location": "<span class=\"SL_block\">United States</span><span class='notranslate'> - </span><span class=\"SL_block\">SC</span>",
  "country_code": 223,
  "location_state": "SC",
  "online": false,
  "availability": "Available",
  "badge_count": 17,
  "badge_level": 2,
  "badge_layout": {
    "badge-11417-2": {
      "creator_id": 11417,
      "creator_badge_index": 2,
      "name": "06-30-09",
      "image_mogilekey": "/userdata/11417/badge_b9010d416397b0130f780ce019e6cc00.gif",
      "image_width": 40,
      "image_height": 20,
      "description": "<img width=\"400\" height=\"162\" src=\"http://static.imvu.com/imvufiles/03_06_09_info.jpg\" alt=\"03_06_09_info.jpg\" />",
      "allow_autogrant": "1",
      "badge_type": "0",
      "review_status": "passed_review",
      "flagger_id": "0",
      "flag_time": "0000-00-00 00:00:00",
      "badgeid": "badge-11417-2",
      "image_url": "http://userimages05.imvu.com/userdata/11417/badge_b9010d416397b0130f780ce019e6cc00.gif",
      "xloc": 0,
      "yloc": 20
    },
    "badge-12861773-3": {
      "creator_id": 12861773,
      "creator_badge_index": 3,
      "name": "Music",
      "image_mogilekey": "/userdata/12861773/badge_1abf3871c688d1eb4d42a6e3b5b650b7.gif",
      "image_width": 20,
      "image_height": 20,
      "description": "<center><b>Customer Appreciation Badge</b></center>\n\nThis badge was created as a small token of my thanks and appreciation for my customers' support. As always, thank you very much for your support &amp; have a fabulous day! :-) ~Goldi\n <center>\n<a href=\"http://avatars.imvu.com/Goldilocks\" target=\"_new\"><img src=\"http://userimages-akm.imvu.com/userdata/12/86/17/73/userpics/Snap_fyRbSjaDWz890693073.gif\" alt=\"Snap_fyRbSjaDWz890693073.gif\"/></a><br/><br/></center> \n \n<center>\n<a href=\"http://www.imvu.com/shop/web_search.php?manufacturers_id=12861773\" target=\"_new\"><img src=\"http://userimages-akm.imvu.com/userdata/12/86/17/73/userpics/goldibadgebanner_0.png\" alt=\"goldibadgebanner_0.png\"/></a>\n</center> \n",
      "allow_autogrant": "1",
      "badge_type": "0",
      "review_status": "passed_review",
      "flagger_id": "0",
      "flag_time": "0000-00-00 00:00:00",
      "badgeid": "badge-12861773-3",
      "image_url": "http://userimages03.imvu.com/userdata/12861773/badge_1abf3871c688d1eb4d42a6e3b5b650b7.gif",
      "xloc": 20,
      "yloc": 0
    },
    "badge-32910455-1": {
      "creator_id": 32910455,
      "creator_badge_index": 1,
      "name": "Jewelzies Bling",
      "image_mogilekey": "/userdata/32910455/badge_74347fbe9b54a934222bd8477ca69c4a.gif",
      "image_width": 20,
      "image_height": 20,
      "description": "<a href=\"http://www.imvu.com/shop/web_search.php?manufacturers_id=32910455\" target=\"_new\"><img src=\"http://img511.imageshack.us/img511/4272/banneranimatedla2.gif\" alt=\"banneranimatedla2.gif\"/></a>",
      "allow_autogrant": "1",
      "badge_type": "0",
      "review_status": "not_reviewed",
      "flagger_id": "0",
      "flag_time": "0000-00-00 00:00:00",
      "badgeid": "badge-32910455-1",
      "image_url": "http://userimages01.imvu.com/userdata/32910455/badge_74347fbe9b54a934222bd8477ca69c4a.gif",
      "xloc": 0,
      "yloc": 0
    },
    "badge-111527836-1": {
      "creator_id": 111527836,
      "creator_badge_index": 1,
      "name": "IMVU Content Creator Tier 6 Badge",
      "image_mogilekey": "/userdata/111527836/badge_bd01d4537e180682cb005a4e7d35437a.gif",
      "image_width": 20,
      "image_height": 20,
      "description": "<div><div><div><a href=\"http://avatars.imvu.com/IMVU%20Badger\" target=\"_new\">What are badges?</a><a href=\"\" target=\"_new\"><img src=\"/common/img/abuse_14x.png\" alt=\"abuse_14x.png\"/>Flag for review</a><a href=\"\" target=\"_new\">X</a><div/></div><div><img src=\"/common/img/icons/badge_tier_level_6.gif\" alt=\"badge_tier_level_6.gif\"/> <b>IMVU Content Creator Tier 6 Badge</b><br/>This badge signifies that the holder has attained Tier Level 6 as an IMVU Creator.</div></div></div>",
      "allow_autogrant": "0",
      "badge_type": "0",
      "review_status": "not_reviewed",
      "flagger_id": "0",
      "flag_time": "0000-00-00 00:00:00",
      "badgeid": "badge-111527836-1",
      "image_url": "http://userimages01.imvu.com/userdata/111527836/badge_bd01d4537e180682cb005a4e7d35437a.gif",
      "xloc": 420,
      "yloc": 80
    }
  },
  "badge_area_html": "<img id='badge-11417-2' class='badgeimg' src='http://userimages05.imvu.com/userdata/11417/badge_b9010d416397b0130f780ce019e6cc00.gif' alt='06-30-09' title='06-30-09' style='width:40px; height:20px; left:0px; top:20px'/><img id='badge-12861773-3' class='badgeimg' src='http://userimages03.imvu.com/userdata/12861773/badge_1abf3871c688d1eb4d42a6e3b5b650b7.gif' alt='Music' title='Music' style='width:20px; height:20px; left:20px; top:0px'/><img id='badge-32910455-1' class='badgeimg' src='http://userimages01.imvu.com/userdata/32910455/badge_74347fbe9b54a934222bd8477ca69c4a.gif' alt='Jewelzies Bling' title='Jewelzies Bling' style='width:20px; height:20px; left:0px; top:0px'/><img id='badge-111527836-1' class='badgeimg' src='http://userimages01.imvu.com/userdata/111527836/badge_bd01d4537e180682cb005a4e7d35437a.gif' alt='IMVU Content Creator Tier 6 Badge' title='IMVU Content Creator Tier 6 Badge' style='width:20px; height:20px; left:420px; top:80px'/>",
  "show_ap": 1,
  "show_flag_icon": 1,
  "show_flag_av": 1,
  "show_message": 1,
  "avpic_default": 0,
  "show_block": true,
  "welcome_moderator_score": 0,
  "is_welcome_moderator": 0,
  "public_rooms": [],
  "visible_albums": 1,
  "show_marriage": 1,
  "married_to_partner_cid": "8647128",
  "married_to_partner_avname": "Haikili",
  "married_to_partner_url": "http://avatars.imvu.com/Haikili",
  "imvu_level": 9,
  "wallpaper_id": "8",
  "status": "success"
};
	render() {
  	return (
    	<div className="app">
        <div><TitleBar imvu_level={this.state.imvu_level} avname={this.state.avname} /></div>
        <div><Body content={this.state} /></div>
      </div>
    );
  };	
}


ReactDOM.render(<App />, mountNode);
