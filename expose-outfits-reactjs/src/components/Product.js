import './Product.css'

const Product = (props) => {

    let apHidden = props.apHidden
    let productId = props.product.product_id
    let productName = props.product.product_name
    let creatorId = props.product.creator_cid
    let creatorName = props.product.creator_name
    let rating = props.product.rating
    let productImage = props.product.product_image
    let isVisible = props.product.is_visible
    let prodUrl = `https://www.imvu.com/shop/product.php?products_id=${productId}`
    if (!isVisible) prodUrl = `https://www.imvu.com/shop/derivation_tree.php?products_id=${productId}`

    let rxVB = /vb|voice|loud|scream|noise|sound|box|chat|effect|grito/ig
    let isLikelyVB = rxVB.test(productName)

    const showTriggers = async (e) => {
        console.log(e)
        let productId = e.target.id.replace('lips-', '')
        let urlPart = process.env.NODE_ENV === 'production' ? '' : 'http://localhost:8080/'
        const productsResponse = await fetch(`${urlPart}productsounds.php?pid=${productId}`);
        const productsData = await productsResponse.json();
        console.log(productsData)

        alert(`"Play Triggers" new feature coming soon. You clicked Product ID ${productId}, which has ${productsData.Triggers.length} triggers`)
    }

    if (creatorName) {
        let cnSplits = creatorName.split('_')
        if (cnSplits.length > 1) {
            creatorName = cnSplits[0] + ' [deleted]'
        }
    }
    else
        creatorName = 'Unknown'

    let apButton = rating === 'AP' ? (<span className="ap">AP</span>) : (<></>)
    let lips = isLikelyVB ? (<img id={'lips-' + productId} className='lips' src="lips.png" onClick={showTriggers}/>) : (<></>)
    let jsx = (
        <div className='product-ctr'>
            <div>
                {apButton}
                <a rel="noreferrer" target="_blank" href={prodUrl}>
                    {productName}</a><br/>by&nbsp; 
                    <a rel="noreferrer" target="_blank" href={`https://www.imvu.com/shop/web_search.php?manufacturers_id=${creatorId}`}>
                        {creatorName}</a>
                    &nbsp;({!isVisible ? 'Hidden' : rating})</div>
            <div className="product-img">
                <a rel="noreferrer" target="_blank" href={prodUrl}>
                <img src={productImage} alt={productName + " by " + creatorName} />
                </a>
                {lips}
            </div>
        </div>
    )

    if (!apHidden) return jsx
    if (rating === "AP" || !isVisible || isLikelyVB) return jsx
    return (<></>)
}

export default Product
