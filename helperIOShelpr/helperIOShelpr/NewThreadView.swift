//
//  NewThreadView.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/5/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit

class NewThreadView: UIViewController {

    @IBOutlet weak var thread: UITextField!
    
    @IBOutlet weak var message: UITextField!
    
    var threadText: String = ""
    var messageText: String = ""
    var wasRecentView: Bool = false
    
    @IBAction func onTouch(_ sender: Any) {
        threadText = thread.text!
        messageText = message.text!

    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

    // Do any additional setup after loading the view.

    }
    
    override func viewWillDisappear(_ animated: Bool) {
        wasRecentView = true
        super.viewWillDisappear(animated)

    }
 
    

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
