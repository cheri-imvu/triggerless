
const Header = (props) => {
    return (
        <>
            <div style={{marginTop: "17px"}}>
                <a href="https://triggerless.com/web">
                    <img src="https://triggerless.com/web/wp-content/uploads/2015/04/logo-u.png" alt="Triggerless homepage" title="Triggerless Home" />
                </a>
            </div>
            <h2 style={headingStyle}>{props.title}</h2>
            <div style={{fontSize: '1.3em', color: 'yellow', backgroundColor: 'black'}}>The fastest, most reliable IMVU Outfit Viewer on the Internet!</div>
        </>
    )
}

Header.defaultProps = {
    title: "Expose IMVU Outfits"
}

const headingStyle = {
    color: "powderblue", 
    backgroundColor: "transparent"
}

export default Header
