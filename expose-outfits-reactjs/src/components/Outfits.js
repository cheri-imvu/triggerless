
import Avatar from "./Avatar"
import { useState } from 'react'
import './Outfits.css'
import Room from "./Room"

const Outfits = () => {

    const [state, setState] = useState({avatars: [], room: [], apHidden: false})

    const getApHidden = () => {
        let apHidden = document.getElementById("apHidden")
        return apHidden.checked
    }

    const clearLink = () => {
        document.getElementById("webLink").value = ''
        setState({apHidden: state.apHidden, avatars: [], room: []})
        console.log(state)
    }
        
    const processLink = async () => {
        let webLink = document.getElementById("webLink").value;
        let linkData = getLinkData(webLink);
        let newState = { avatars: [], room: state.room, apHidden: getApHidden() };
        if (linkData.avatars == null) {
            setState(newState);
            return;
        }
    
        let urlPart = process.env.NODE_ENV === 'production' ? '' : 'http://localhost:8080/';

        const getProducts = async (productIDs) => {
            const queryString = 'p=' + productIDs.join('&p=');
            const productsResponse = await fetch(`${urlPart}products.php?${queryString}`);
            const productsData = await productsResponse.json();
            return productsData.Products.filter(p => p.product_name != null); // remove hidden products
        }

        // get the room first
        
        const roomProductList = linkData.room.map(id => id.split('x')[0]) //gets rid of x2 on the end
        const roomData = await getProducts(roomProductList);

        setState(prevState => ({
            ...prevState,
            room: [...prevState.room, roomData]
        }));
        console.log(state)
        /*        
        const roomState = {
            avatars: [...state.avatars],
            room: [...state.room, roomData],
            apHidden: getApHidden()
        }
        console.log(roomState)
        setState(roomState)
        */
        
    
        // Concurrency limit logic
        const limit = 10; // Limit to 10 concurrent fetches
        const fetchWithLimit = async (linkDataAvi) => {
            const userResponse = await fetch(`${urlPart}user.php?id=${linkDataAvi.id}`);
            const currentUser = await userResponse.json();
    
            currentUser.products = await getProducts(linkDataAvi.products) 

             // Use functional form of setState to ensure you always work with the latest state
            setState(prevState => ({
                ...prevState,
                avatars: [...prevState.avatars, currentUser]
            }));
            console.log(state)
            /*
            newState = { 
                avatars: [...newState.avatars, currentUser], 
                room: [...state.room], 
                apHidden: getApHidden() 
            };
            console.log(newState);
            setState(newState);
            */
        };
    
        const chunkArray = (array, size) => {
            const chunks = [];
            for (let i = 0; i < array.length; i += size) {
                chunks.push(array.slice(i, i + size));
            }
            return chunks;
        };
    
        // Process avatars in chunks to respect the concurrency limit
        const avatarChunks = chunkArray(linkData.avatars, limit);
        for (const chunk of avatarChunks) {
            await Promise.all(chunk.map(fetchWithLimit));
        }
    };
    

    return (
        <>
        <div>How to use this...</div>
        <ol>
            <li>In IMVU Client, right-click anywhere in the room, and select "View products in this scene".</li>
            <li>Your browser should pop up with a page. Usually, many of the outfits will be hidden.</li>
            <li>Go to the Address Bar near the top, and select the entire web link. Copy it to the clipboard.</li>
            <li>If you want to view only AP items, Voice Boxes and Hidden Products, tick the check box.</li>
            <li>Paste the web link into the text box below. Click "Expose" button. After a brief wait, the outfits will be exposed.</li>
        </ol>
        <table>
            <tbody>
            <tr>
                <td>
                <div style={{paddingLeft: "25px"}}>
                    <textarea id="webLink" style={{width: "500px", height: "200px"}} placeholder="Paste web link here"></textarea>
                </div>
                <div style={{paddingLeft: "25px"}}>
                    <input type="checkbox" id="apHidden" onChange={processLink} />
                    <label htmlFor="apHidden">AP, VBs &amp; Hidden Only</label>
                </div>
                </td>
                <td>
                <div><button className="outfits-btn" onClick={processLink}>Expose</button></div>
                <div><button className="outfits-btn" onClick={clearLink}>Clear</button></div>
                </td>
            </tr>
            <tr>
                <td colSpan="2" style={{fontSize: "0.8em"}}>Please note: This will only work on browsers released later than 2015. 
                    If your browser is older, this isn't guaranteed to work.</td>
            </tr>
            </tbody>
        </table>
        <a name="avatars" />
        <div className="avis-ctr">
            <strong>Avatars:</strong> {
                state
                    .avatars
                    .sort((a, b) => {
                        let fa = a.avatarname.toLowerCase(), fb = b.avatarname.toLowerCase()
                        return (fa < fb) ? -1 : (fb < fa) ? 1 : 0
                    })
                    .map((avi) => (
                        <a className="avis" key={avi.id} href={'#' + avi.avatarname}>{' ' + avi.avatarname}</a> 
                    ))
                }
        </div>
        <div>
            <Room key="room-ctr" products={state.room[0]} />
        </div>
        <div>
            {state
                .avatars
                .sort((a, b) => {
                    let fa = a.avatarname.toLowerCase(), fb = b.avatarname.toLowerCase()
                    return (fa < fb) ? -1 : (fb < fa) ? 1 : 0
                })
                
                .map((avi) => (
                    <Avatar key={'avi-' + avi.id} avatar={avi} apHidden={state.apHidden} />
                ))
            }
        </div>
        </>
    )
}

const getLinkData = (str) => {
    let linkData = {
        status: 'failure',
        message: 'Sorry, that link won\'t work. Please try again.'
    }

    str = str.replace('\n', '').replace(' '); // remove all whitespace

    //snip1 URL and query string

    let snip1 = str.split('?');
    if (snip1.length !== 2) return linkData;

    // snip2 query parameters

    let snip2 = snip1[1].split('&');
    if (snip2.length < 2) return linkData; // at least avatar and room should be available

    linkData.avatars = [];
    linkData.room = [];

    for (let i = 0; i < snip2.length; i++) {
        // snip3 name and value

        let snip3 = snip2[i].split('=');
        if (snip3.length !== 2) continue;

        if (snip3[0].indexOf('avatar') === 0) {
            linkData.avatars.push({
                id: snip3[0].replace('avatar', ''),
                products: snip3[1].split('%3B')
            });
        } else if (snip3[0] === 'room') {
            linkData.room = snip3[1].split('%3B').map(p => p.split('x')[0]);
        } else continue;
    }

    linkData.status = 'success';
    linkData.message = `Decoded room and ${linkData.avatars.length} avatars.`
    return linkData;

}
export default Outfits
