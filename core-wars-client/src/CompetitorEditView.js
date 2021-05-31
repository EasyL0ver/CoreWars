import React from "react";

import Select from 'react-select'



class CompetitorEditView extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            selected: null
        }
     
    }

    componentDidUpdate(prevProps, prevState) {
        if(prevProps.categories != this.props.categories)
            this.setState({
                ...this.state,
                selected: null,
            });
     }

    onSubmit(){
        this.props.onSubmit("alias", "kod")
    }

    changeCompetition(competition){
        this.setState({
            ...this.state,
            selected: competition,
        });
    }

    render(){

        let selected = this.state.selected

        if(selected == null && this.props.categories.length == 1)
            selected = this.props.categories[0]

        return ( 
        <div> 
            <form>
                <input type="text" id="fname" name="fname" value={this.props.alias}/><br/><br/>
            </form>
            <Select options={this.props.categories} value={selected} onChange={this.changeCompetition.bind(this)}/>
            <textarea value={this.props.code}></textarea>
            <button onClick={this.onSubmit}>SUBMIT</button>
        </div>)
    }

}


export default CompetitorEditView;