import './Avatar.css'
import Product from './Product'

const Avatar = (props) => {
    let name = props.avatar.avatarname
    let photo = props.avatar.photo
    let products = props.avatar.products
    let apHidden = props.apHidden

    return (
        <div className="av-ctr">
            <div>
                <a href="#avatars">
                    <img className='av-point-up' src="point-up.png" alt='Jump to Top' />
                </a>
                <div className="av-name">{name}
                    <a name={name} />
                </div>

                <div className="av-photo">
                    <a target="_blank" rel="noreferrer" href={"https://avatars.imvu.com/" + name} >
                    <img src={photo} className="img-photo" alt={'image of ' + name}/>
                    </a>
                </div>
                {products.map((prod) => (
                    <Product key={'prod-' + prod.product_id} product={prod} apHidden={apHidden} />
                ))}

            </div>
            <div>
            </div>
        </div>
    )
}

export default Avatar

