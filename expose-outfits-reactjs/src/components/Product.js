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

    let cnSplits = creatorName.split('_')
    if (cnSplits.length > 1) {
        creatorName = cnSplits[0] + ' [deleted]'
    }

    let apButton = rating === 'AP' ? (<span class="ap">AP</span>) : (<></>)
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
            </div>
        </div>
    )

    if (!apHidden) return jsx
    let rxVB = /vb|voice|loud|scream|noise|sound/ig
    if (rating === "AP" || !isVisible || rxVB.test(productName)) return jsx
    return (<></>)
}

export default Product
