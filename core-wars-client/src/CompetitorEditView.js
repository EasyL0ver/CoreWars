import React from "react";
import axios from "axios";


class CompetitorEditView extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitorState: {
                gamesPlayed:0 ,
                
            }
        }
    }

    onSubmit(){
        this.props.onSubmit("alias", "kod")
    }

    render(){
        return ( 
        <div> 
            <form>
                <input type="text" id="fname" name="fname" value={this.props.alias}/><br/><br/>
            </form>
            <textarea value={this.props.code}></textarea>
            <button onClick={this.onSubmit}>SUBMIT</button>
        </div>)
    }

}


export default CompetitorEditView;