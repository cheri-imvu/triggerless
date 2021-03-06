import Header from './components/Header'
import Outfits from './components/Outfits'
import './App.css'

const App = () => {
  return (
    <div className="app-ctr">
      <Header title="Expose Hidden IMVU Outfits" />
      <Outfits />
    </div>
  )
}

export default App;
