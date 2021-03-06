import './Room.css';
import Product from './Product';

const Room = (props) => {

    let name = "Room";
    if (!props.products) return (
        <></>
    );
    let products = props.products;

    return (
        <div className="room-ctr">
            <div>
                <div className="room-name">{name}</div>
                {products.map((prod) => (
                    <Product key={prod.product_id} product={prod} />
                ))}

            </div>
        </div>
    )}

export default Room
