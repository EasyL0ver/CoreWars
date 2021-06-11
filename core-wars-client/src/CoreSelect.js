import React from "react";

class CoreSelect extends React.Component {
    constructor(props) {
        super(props)
    }

    selectChanged(event){
        const value = event.target.value
        this.props.onChange(value)
    }

    render() {
        const options = this.props.options.map(option => {
            return (
                <option value={option.value} key={option.value}>{option.label}</option>
            )
        })

        return (
            <select value={this.props.value} onChange={this.selectChanged.bind(this)}>
                {options}
            </select>
        )
    }
}


export default CoreSelect;