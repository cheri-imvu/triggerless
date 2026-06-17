import Avatar from "./Avatar"
import { useState } from 'react'
import './Outfits.css'
import Room from "./Room"
import TriggerView from "./TriggerView"

/* -------------------- CORS PROXY ROTATION -------------------- */

const corsProxies = [
    // corsproxy.io
    (url) => `https://corsproxy.io/?url=${encodeURIComponent(url)}`,

    // thingproxy (sometimes flaky, but fast when it works)
    (url) => `https://thingproxy.freeboard.io/fetch/${url}`,

    // allorigins (JSON wrapper, slower but reliable)
    (url) => `https://api.allorigins.win/raw?url=${encodeURIComponent(url)}`
]

const pickProxy = () =>
    corsProxies[Math.floor(Math.random() * corsProxies.length)]

const timedFetch = async (url) => {
    const proxy = pickProxy()
    const proxiedUrl = proxy(url)

    const start = performance.now()
    const res = await fetch(proxiedUrl)
    const end = performance.now()

    if (process.env.NODE_ENV !== 'production') {
        console.log(
            `[FETCH] ${Math.round(end - start)}ms → ${proxiedUrl}`
        )
    }

    return res
}

/* -------------------- COMPONENT -------------------- */

const Outfits = () => {

    const [state, setState] = useState({
        avatars: [],
        room: [],
        apHidden: false
    })

    const getApHidden = () =>
        document.getElementById("apHidden").checked

    const clearLink = () => {
        document.getElementById("webLink").value = ''
        setState({ avatars: [], room: [], apHidden: state.apHidden })
    }

    /* -------------------- API FETCHERS -------------------- */

    const fetchUser = async (userId) => {
        const url = `https://api.imvu.com/user/user-${userId}`
        const res = await timedFetch(url)
        const json = await res.json()

        const key = Object.keys(json.denormalized)[0]
        return json.denormalized[key].data
    }

    const fetchProduct = async (productId) => {
        const url = `https://api.imvu.com/product/product-${productId}`
        const res = await timedFetch(url)
        const json = await res.json()

        const key = Object.keys(json.denormalized)[0]
        return json.denormalized[key].data
    }

    const fetchProducts = async (productIds) => {
        const requests = productIds.map(id =>
            fetchProduct(id).catch(() => null)
        )

        const results = await Promise.all(requests)

        return results.filter(p => p && p.product_name != null)
    }

    /* -------------------- MAIN FLOW -------------------- */

    const processLink = async () => {
        const webLink = document.getElementById("webLink").value
        const linkData = getLinkData(webLink)

        if (!linkData.avatars) {
            setState({ avatars: [], room: [], apHidden: getApHidden() })
            return
        }

        setState({
            avatars: [],
            room: [],
            apHidden: getApHidden()
        })

        /* ---------- ROOM ---------- */

        const roomProducts = await fetchProducts(
            linkData.room.map(p => p.split('x')[0])
        )

        setState(prev => ({
            ...prev,
            room: [roomProducts]
        }))

        /* ---------- AVATARS ---------- */

        for (const avi of linkData.avatars) {
            const user = await fetchUser(avi.id)

            user.products = await fetchProducts(
                avi.products.map(p => p.split('x')[0])
            )

            setState(prev => ({
                ...prev,
                avatars: [...prev.avatars, user]
            }))
        }
    }

    return (
        <>
            <div>How to use this...</div>
            <TriggerView />

            <textarea
                id="webLink"
                style={{ width: 600, height: 200 }}
                placeholder="Paste web link here"
            />

            <div>
                <input type="checkbox" id="apHidden" onChange={processLink} />
                <label htmlFor="apHidden">AP, VBs & Hidden Only</label>
            </div>

            <button className="outfits-btn" onClick={processLink}>Expose</button>
            <button className="outfits-btn" onClick={clearLink}>Clear</button>

            <div className="avis-ctr">
                <strong>Avatars:</strong>
                {state.avatars.map(avi => (
                    <a key={avi.legacy_cid} className="avis">
                        {avi.avatarname}
                    </a>
                ))}
            </div>

            <Room products={state.room[0]} />

            {state.avatars.map(avi => (
                <Avatar
                    key={'avi-' + avi.legacy_cid}
                    avatar={avi}
                    apHidden={state.apHidden}
                />
            ))}
        </>
    )
}

/* -------------------- LINK PARSER -------------------- */

const getLinkData = (str) => {
    str = str.replace(/\s/g, '')
    const result = { avatars: [], room: [] }

    const parts = str.split('?')
    if (parts.length !== 2) return result

    for (const param of parts[1].split('&')) {
        const [key, value] = param.split('=')
        if (!value) continue

        if (key.startsWith('avatar')) {
            result.avatars.push({
                id: key.replace('avatar', ''),
                products: value.split('%3B')
            })
        } else if (key === 'room') {
            result.room = value.split('%3B')
        }
    }

    return result
}

export default Outfits
