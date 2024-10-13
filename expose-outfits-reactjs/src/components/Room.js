import './Room.css';
import Product from './Product';

const Room = (props) => {

    let name = "This Room"

    let products = props.products
    let apHidden = props.apHidden
    if (!products) return (
        <></>
    );

    return (
        <div className="room-ctr">
            <div>
                <div className="room-name">{name}</div>
                {products.map((prod) => (
                    <Product key={ 'prod-' + prod.product_id} product={prod} apHidden={apHidden} />
                ))}

            </div>
        </div>
    )}

export default Room
