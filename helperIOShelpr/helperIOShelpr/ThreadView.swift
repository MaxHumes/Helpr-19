//
//  ThreadView.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/5/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import CoreData

class ThreadView: UIView {


    @IBOutlet weak var thread: UITextField!
    
    @IBOutlet weak var message: UITextField!
    
    var threadText: String = ""
    var messageText: String = ""
    
    @IBAction func onTouch(_ sender: Any) {
        threadText = thread.text!
        messageText = message.text!
    }
    /*
    // Only override draw() if you perform custom drawing.
    // An empty implementation adversely affects performance during animation.
    override func draw(_ rect: CGRect) {
        // Drawing code
    }
    */

}
